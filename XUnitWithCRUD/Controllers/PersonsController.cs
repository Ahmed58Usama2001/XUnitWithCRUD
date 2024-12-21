using XUnitWithCRUD.Filters.ActionFilters;
using XUnitWithCRUD.Filters.ResultFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.DTO.Enums;

namespace XUnitWithCRUD.Controllers;
[Route("[controller]")]
[TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] { "My-Key-From-Controller", "My-Value-From-Controller" })]
public class PersonsController : Controller
{
    private readonly IPersonsService _personService;
    private readonly ICountriesService _countriesService;
    private readonly ILogger<PersonsController> _logger;

    public PersonsController(IPersonsService personService, ICountriesService countriesService, ILogger<PersonsController> logger)
    {
        _personService = personService;
        _countriesService = countriesService;
        _logger = logger;
    }
    [Route("[action]")]
    [Route("/")]
    [TypeFilter(typeof(PersonsListActionFilter))]
    [TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] { "MyKey-FromAction", "MyValue-From-Action" })]
    [TypeFilter(typeof(PersonsListResultFilter))]

    public async Task<IActionResult> Index(string searchBy , string? searchString,
        string sortBy = nameof(PersonResponse.PersonName) , SortOrderOptions sortOrder = SortOrderOptions.ASC)
    {
        _logger.LogInformation("Index action method of PersonsController");
        _logger.LogDebug($"searchBy: {searchBy}, searchString: {searchString}, sortBy: {sortBy}, sortOrder: {sortOrder}");

        List<PersonResponse> persons =await _personService.GetFilteredPersons(searchBy , searchString);

        List<PersonResponse> sortedPersons =await _personService.GetSortedPersons(persons , sortBy , sortOrder);

        return View(sortedPersons);
    }

    [Route("[action]")]
    [HttpGet]
    public async Task<IActionResult> Create()
    {


        List<CountryResponse> countries =await _countriesService.GetAllCountries();
        ViewBag.Countries = countries.Select(c => new SelectListItem
        {
            Text = c.CountryName,
            Value = c.CountryId.ToString()
        });

        return View();
    }

    [Route("[action]")]
    [HttpPost]
    [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
    public async Task<IActionResult> Create(PersonAddRequest request)
    {   
        PersonResponse personResponse =await _personService.AddPerson(request);

        return RedirectToAction("Index", "Persons");
    }

    [HttpGet]
    [Route("[action]/{personId}")] 
    public async Task<IActionResult> Edit(Guid personId)
    {
        PersonResponse? personResponse =await _personService.GetPersonByPersonId(personId);
        if (personResponse == null)
        {
            return RedirectToAction("Index");
        }

        PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

        List<CountryResponse> countries = await _countriesService.GetAllCountries();
        ViewBag.Countries = countries.Select(temp =>
        new SelectListItem() { Text = temp.CountryName, Value = temp.CountryId.ToString() });

        return View(personUpdateRequest);
    }

    [HttpPost]
    [Route("[action]/{PersonId}")]
    [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
    public async Task<IActionResult> Edit(PersonUpdateRequest personUpdateRequest)
    {
        PersonResponse? personResponse =await _personService.GetPersonByPersonId(personUpdateRequest.PersonId);

        if (personResponse == null)
        {
            return RedirectToAction("Index");
        }

        PersonResponse updatedPerson =await _personService.UpdatePerson(personUpdateRequest);
        return RedirectToAction("Index");
    }


    [HttpGet]
    [Route("[action]/{personId}")]
    public async Task<IActionResult> Delete(Guid? personId)
    {
        PersonResponse? personResponse =await _personService.GetPersonByPersonId(personId);
        if (personResponse == null)
            return RedirectToAction("Index");

        return View(personResponse);
    }

    [HttpPost]
    [Route("[action]/{personId}")]
    public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateResult)
    {
        PersonResponse? personResponse =await _personService.GetPersonByPersonId(personUpdateResult.PersonId);
        if (personResponse == null)
            return RedirectToAction("Index");

        await _personService.DeletePerson(personUpdateResult.PersonId);
        return RedirectToAction("Index");
    }

    [Route("PersonsPDF")]
    public async Task<IActionResult> PersonsPDF()
    {
        List<PersonResponse> persons = await _personService.GetAllPersons();

        return new ViewAsPdf("PersonsPDF", persons, ViewData)
        {
            PageMargins = new Rotativa.AspNetCore.Options.Margins() { Top = 20, Right = 20, Bottom = 20, Left = 20 },
            PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape
        };
    }

    [Route("PersonsExcel")]
    public async Task<IActionResult> PersonsExcel()
    {
        MemoryStream memoryStream = await _personService.GetPersonsExcel();
        return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "persons.xlsx");
    }
}
