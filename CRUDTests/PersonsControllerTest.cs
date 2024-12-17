using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.DTO.Enums;
using Services;
using XUnitWithCRUD.Controllers;

namespace CRUDTests;

public class PersonsControllerTest
{
    private readonly Fixture _fixture;
    private readonly IPersonsService _personsService;
    private readonly ICountriesService _countriesService;
    private readonly Mock<IPersonsService> _personsServiceMock;
    private readonly Mock<ICountriesService> _countriesServiceMock;

    public PersonsControllerTest()
    {
        _fixture = new Fixture();

        _countriesServiceMock = new Mock<ICountriesService>();
        _personsServiceMock = new Mock<IPersonsService>();

        _personsService = _personsServiceMock.Object;
        _countriesService = _countriesServiceMock.Object;
    }

    #region Index
    [Fact]
    public async Task Index_ShouldReturnIndexViewWithPersonsList()
    {
        List<PersonResponse> personsResponseList = _fixture.Create<List<PersonResponse>>();

        PersonsController personsController = new PersonsController(_personsService, _countriesService);

        _personsServiceMock.Setup(temp=> temp.GetFilteredPersons(It.IsAny<string>() , It.IsAny<string>()))
            .ReturnsAsync(personsResponseList);

        _personsServiceMock.Setup(temp=> temp.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>() , It.IsAny<SortOrderOptions>()))
            .ReturnsAsync(personsResponseList);

        IActionResult result = await personsController.Index(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<SortOrderOptions>());

       ViewResult viewResult = Assert.IsType<ViewResult>(result);

        viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
        viewResult.ViewData.Model.Should().Be(personsResponseList);

    }
    #endregion


    #region Create

    [Fact]
    public async void Create_IfModelErrors_ToReturnCreateView()
    {
        PersonAddRequest person_add_request = _fixture.Create<PersonAddRequest>();

        PersonResponse person_response = _fixture.Create<PersonResponse>();

        List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

        _countriesServiceMock
         .Setup(temp => temp.GetAllCountries())
         .ReturnsAsync(countries);

        _personsServiceMock
         .Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
         .ReturnsAsync(person_response);

        PersonsController personsController = new PersonsController(_personsService, _countriesService);


        personsController.ModelState.AddModelError("PersonName", "Person Name can't be blank");

        IActionResult result = await personsController.Create(person_add_request);

        ViewResult viewResult = Assert.IsType<ViewResult>(result);

        viewResult.ViewData.Model.Should().BeAssignableTo<PersonAddRequest>();

        viewResult.ViewData.Model.Should().Be(person_add_request);
    }


    [Fact]
    public async void Create_IfNoModelErrors_ToReturnRedirectToIndex()
    {
        PersonAddRequest person_add_request = _fixture.Create<PersonAddRequest>();

        PersonResponse person_response = _fixture.Create<PersonResponse>();

        List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

        _countriesServiceMock
         .Setup(temp => temp.GetAllCountries())
         .ReturnsAsync(countries);

        _personsServiceMock
         .Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
         .ReturnsAsync(person_response);

        PersonsController personsController = new PersonsController(_personsService, _countriesService);


        IActionResult result = await personsController.Create(person_add_request);

        RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);

        redirectResult.ActionName.Should().Be("Index");
    }

    #endregion
}
