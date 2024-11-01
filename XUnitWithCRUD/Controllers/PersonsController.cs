using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.DTO.Enums;
using Services;
using System.Collections.Generic;

namespace XUnitWithCRUD.Controllers;
[Route("[controller]")]
public class PersonsController : Controller
{
    private readonly IPersonService _personService;
    private readonly ICountriesService _countriesService;

    public PersonsController(IPersonService personService, ICountriesService countriesService)
    {
        _personService = personService;
        _countriesService = countriesService;
    }
    [Route("[action]")]
    [Route("/")]
    public IActionResult Index(string searchBy , string? searchString,
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

        List<PersonResponse> persons = _personService.GetFilteredPersons(searchBy , searchString);
        ViewBag.CurrentSearchBy = searchBy;
        ViewBag.CurrentSearchString = searchString;

        List<PersonResponse> sortedPersons = _personService.GetSortedPersons(persons , sortBy , sortOrder);
        ViewBag.CurrentSortBy = sortBy;
        ViewBag.CurrentSortOrder = sortOrder.ToString();


        return View(sortedPersons);
    }

    [Route("[action]")]
    [HttpGet]
    public IActionResult Create()
    {
        List<CountryResponse> countries = _countriesService.GetAllCountries();
        ViewBag.Countries = countries;

        return View();
    }

    [Route("[action]")]
    [HttpPost]
    public IActionResult Create(PersonAddRequest request)
    {
        if (!ModelState.IsValid)
        {
            List<CountryResponse> countries = _countriesService.GetAllCountries();
            ViewBag.Countries = countries;

            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return View();
        }

        PersonResponse personResponse = _personService.AddPerson(request);

        return RedirectToAction("Index", "Persons");
    }
}
