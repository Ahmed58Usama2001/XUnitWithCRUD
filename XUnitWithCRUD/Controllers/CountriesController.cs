using Microsoft.AspNetCore.Mvc;

namespace XUnitWithCRUD.Controllers;

[Route("[controller]")]
public class CountriesController : Controller
{
    [Route("UploadFromExcel")]
    public IActionResult UploadFromExcel()
    {
        return View();
    }
}