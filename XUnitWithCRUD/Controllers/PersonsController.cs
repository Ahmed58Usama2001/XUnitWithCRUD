using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;

namespace XUnitWithCRUD.Controllers;

public class PersonsController : Controller
{
    private readonly IPersonService _personService;

    public PersonsController(IPersonService personService)
    {
        _personService = personService;
    }
    [Route("persons/index")]
    [Route("/")]
    public IActionResult Index()
    {   List<PersonResponse> persons = _personService.GetAllPersons();
        return View(persons);
    }
}
