using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.DTO.Enums;
using Services;
using System.Collections.Generic;

namespace XUnitWithCRUD.Controllers;
[Route("[controller]")]
public class PersonsController : Controller
{
    private readonly IPersonsService _personService;
    private readonly ICountriesService _countriesService;

    public PersonsController(IPersonsService personService, ICountriesService countriesService)
    {
        _personService = personService;
        _countriesService = countriesService;
    }
    [Route("[action]")]
    [Route("/")]
    public async Task<IActionResult> Index(string searchBy , string? searchString,
        string sortBy = nameof(PersonResponse.PersonName) , SortOrderOptions sortOrder = SortOrderOptions.ASC)
    {
        ViewBag.SearchFields = new Dictionary<string, string>()
         {
            { nameof(PersonResponse.PersonName), "Person Name" },
            { nameof(PersonResponse.Email), "Email" },
            { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
            { nameof(PersonResponse.Gender), "Gender" },
            { nameof(PersonResponse.CountryId), "Country" },
            { nameof(PersonResponse.Address), "Address" }
         };

        List<PersonResponse> persons =await _personService.GetFilteredPersons(searchBy , searchString);
        ViewBag.CurrentSearchBy = searchBy;
        ViewBag.CurrentSearchString = searchString;

        List<PersonResponse> sortedPersons =await _personService.GetSortedPersons(persons , sortBy , sortOrder);
        ViewBag.CurrentSortBy = sortBy;
        ViewBag.CurrentSortOrder = sortOrder.ToString();


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
    public async Task<IActionResult> Create(PersonAddRequest request)
    {
        if (!ModelState.IsValid)
        {
            List<CountryResponse> countries =await _countriesService.GetAllCountries();
            ViewBag.Countries = countries;

            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return View(request);
        }

        PersonResponse personResponse =await _personService.AddPerson(request);

        return RedirectToAction("Index", "Persons");
    }

    [HttpGet]
    [Route("[action]/{personId}")] //Eg: /persons/edit/1
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
    public async Task<IActionResult> Edit(PersonUpdateRequest personUpdateRequest)
    {
        PersonResponse? personResponse =await _personService.GetPersonByPersonId(personUpdateRequest.PersonId);

        if (personResponse == null)
        {
            return RedirectToAction("Index");
        }

        if (ModelState.IsValid)
        {
            PersonResponse updatedPerson =await _personService.UpdatePerson(personUpdateRequest);
            return RedirectToAction("Index");
        }
        else
        {
            List<CountryResponse> countries =await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp =>
            new SelectListItem() { Text = temp.CountryName, Value = temp.CountryId.ToString() });

            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return View();
        }
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
