using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.DTO.Enums;
using Services;
using Xunit.Abstractions;

namespace CRUDTests;

public class PersonsServiceTest
{
    //private fields
    private readonly IPersonsService _personService;
    private readonly ICountriesService _countriesService;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IFixture _fixture;

    //constructor
    public PersonsServiceTest(ITestOutputHelper testOutputHelper)
    {
        _fixture = new Fixture();

        var countriesInitialData = new List<Country>() { };
        var personsInitialData = new List<Person>() { };

        DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
          new DbContextOptionsBuilder<ApplicationDbContext>().Options
         );

        ApplicationDbContext dbContext = dbContextMock.Object;

        dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
        dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);

        _countriesService = new CountriesService(dbContext);

        _personService = new PersonsService(_countriesService,dbContext);

        _testOutputHelper = testOutputHelper;
    }

    #region AddPerson

    
    [Fact]
    public async Task AddPerson_NullPerson()
    {
        PersonAddRequest? personAddRequest = null;

        Func<Task> action = async () =>
        {
            await _personService.AddPerson(personAddRequest);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();
    }


    [Fact]
    public async Task AddPerson_PersonNameIsNull()
    {
        PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
         .With(temp => temp.PersonName, null as string)
         .Create();

        Func<Task> action = async () =>
        {
            await _personService.AddPerson(personAddRequest);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddPerson_ProperPersonDetails()
    {
        PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
         .With(temp => temp.Email, "someone@example.com")
         .Create();

        PersonResponse person_response_from_add = await _personService.AddPerson(personAddRequest);

        List<PersonResponse> persons_list = await _personService.GetAllPersons();

  
        person_response_from_add.PersonId.Should().NotBe(Guid.Empty);

        
        persons_list.Should().Contain(person_response_from_add);
    }

    #endregion


    #region GetPersonByPersonID

    [Fact]
    public async Task GetPersonByPersonID_NullPersonID()
    {
        Guid? personID = null;

        PersonResponse? person_response_from_get = await _personService.GetPersonByPersonId(personID);

        person_response_from_get.Should().BeNull();
    }


    [Fact]
    public async Task GetPersonByPersonID_WithPersonID()
    {
        CountryAddRequest country_request = _fixture.Create<CountryAddRequest>();

        CountryResponse country_response = await _countriesService.AddCountry(country_request);

        PersonAddRequest person_request = _fixture.Build<PersonAddRequest>()
         .With(temp => temp.Email, "email@sample.com")
         .With(temp => temp.CountryId, country_response.CountryId)
         .Create();

        PersonResponse person_response_from_add = await _personService.AddPerson(person_request);

        PersonResponse? person_response_from_get = await _personService.GetPersonByPersonId(person_response_from_add.PersonId);

        person_response_from_get.Should().Be(person_response_from_add);
    }

    #endregion


    #region GetAllPersons

    [Fact]
    public async Task GetAllPersons_EmptyList()
    {
        List<PersonResponse> persons_from_get = await _personService.GetAllPersons();

        persons_from_get.Should().BeEmpty();
    }


    [Fact]
    public async Task GetAllPersons_AddFewPersons()
    {
        CountryAddRequest country_request_1 = _fixture.Create<CountryAddRequest>();
        CountryAddRequest country_request_2 = _fixture.Create<CountryAddRequest>();

        CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
        CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

        PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>()
         .With(temp => temp.Email, "someone_1@example.com")
         .With(temp => temp.CountryId, country_response_1.CountryId)
         .Create();

        PersonAddRequest person_request_2 = _fixture.Build<PersonAddRequest>()
         .With(temp => temp.Email, "someone_2@example.com")
         .With(temp => temp.CountryId, country_response_2.CountryId)
         .Create();

        PersonAddRequest person_request_3 = _fixture.Build<PersonAddRequest>()
         .With(temp => temp.Email, "someone_3@example.com")
         .With(temp => temp.CountryId, country_response_2.CountryId)
         .Create();

        List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };

        List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

        foreach (PersonAddRequest person_request in person_requests)
        {
            PersonResponse person_response = await _personService.AddPerson(person_request);
            person_response_list_from_add.Add(person_response);
        }

        _testOutputHelper.WriteLine("Expected:");
        foreach (PersonResponse person_response_from_add in person_response_list_from_add)
        {
            _testOutputHelper.WriteLine(person_response_from_add.ToString());
        }

        List<PersonResponse> persons_list_from_get = await _personService.GetAllPersons();

        _testOutputHelper.WriteLine("Actual:");
        foreach (PersonResponse person_response_from_get in persons_list_from_get)
        {
            _testOutputHelper.WriteLine(person_response_from_get.ToString());
        }

        persons_list_from_get.Should().BeEquivalentTo(person_response_list_from_add);
    }
    #endregion


    #region GetFilteredPersons

    [Fact]
    public async Task GetFilteredPersons_EmptySearchText()
    {
        CountryAddRequest country_request_1 = _fixture.Create<CountryAddRequest>();
        CountryAddRequest country_request_2 = _fixture.Create<CountryAddRequest>();

        CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
        CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

        PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>()
         .With(temp => temp.Email, "someone_1@example.com")
         .With(temp => temp.CountryId, country_response_1.CountryId)
         .Create();

        PersonAddRequest person_request_2 = _fixture.Build<PersonAddRequest>()
         .With(temp => temp.Email, "someone_2@example.com")
         .With(temp => temp.CountryId, country_response_2.CountryId)
         .Create();

        PersonAddRequest person_request_3 = _fixture.Build<PersonAddRequest>()
         .With(temp => temp.Email, "someone_3@example.com")
         .With(temp => temp.CountryId, country_response_2.CountryId)
         .Create();

        List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };

        List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

        foreach (PersonAddRequest person_request in person_requests)
        {
            PersonResponse person_response = await _personService.AddPerson(person_request);
            person_response_list_from_add.Add(person_response);
        }

        _testOutputHelper.WriteLine("Expected:");
        foreach (PersonResponse person_response_from_add in person_response_list_from_add)
        {
            _testOutputHelper.WriteLine(person_response_from_add.ToString());
        }

        List<PersonResponse> persons_list_from_search = await _personService.GetFilteredPersons(nameof(Person.PersonName), "");

        _testOutputHelper.WriteLine("Actual:");
        foreach (PersonResponse person_response_from_get in persons_list_from_search)
        {
            _testOutputHelper.WriteLine(person_response_from_get.ToString());
        }

        
        persons_list_from_search.Should().BeEquivalentTo(person_response_list_from_add);
    }


    [Fact]
    public async Task GetFilteredPersons_SearchByPersonName()
    {
        //Arrange
        CountryAddRequest country_request_1 = _fixture.Create<CountryAddRequest>();
        CountryAddRequest country_request_2 = _fixture.Create<CountryAddRequest>();

        CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
        CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

        PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>()
         .With(temp => temp.PersonName, "Rahman")
         .With(temp => temp.Email, "someone_1@example.com")
         .With(temp => temp.CountryId, country_response_1.CountryId)
         .Create();

        PersonAddRequest person_request_2 = _fixture.Build<PersonAddRequest>()
         .With(temp => temp.PersonName, "mary")
         .With(temp => temp.Email, "someone_2@example.com")
         .With(temp => temp.CountryId, country_response_1.CountryId)
         .Create();

        PersonAddRequest person_request_3 = _fixture.Build<PersonAddRequest>()
         .With(temp => temp.PersonName, "scott")
         .With(temp => temp.Email, "someone_3@example.com")
         .With(temp => temp.CountryId, country_response_2.CountryId)
         .Create();

        List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };

        List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

        foreach (PersonAddRequest person_request in person_requests)
        {
            PersonResponse person_response = await _personService.AddPerson(person_request);
            person_response_list_from_add.Add(person_response);
        }

        _testOutputHelper.WriteLine("Expected:");
        foreach (PersonResponse person_response_from_add in person_response_list_from_add)
        {
            _testOutputHelper.WriteLine(person_response_from_add.ToString());
        }

        List<PersonResponse> persons_list_from_search = await _personService.GetFilteredPersons(nameof(Person.PersonName), "ma");

        _testOutputHelper.WriteLine("Actual:");
        foreach (PersonResponse person_response_from_get in persons_list_from_search)
        {
            _testOutputHelper.WriteLine(person_response_from_get.ToString());
        }

        persons_list_from_search.Should().OnlyContain(temp => temp.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase));
    }

    #endregion


    #region GetSortedPersons

    [Fact]
    public async Task GetSortedPersons()
    {
        CountryAddRequest country_request_1 = _fixture.Create<CountryAddRequest>();
        CountryAddRequest country_request_2 = _fixture.Create<CountryAddRequest>();

        CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
        CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

        PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>()
         .With(temp => temp.PersonName, "Smith")
         .With(temp => temp.Email, "someone_1@example.com")
         .With(temp => temp.CountryId, country_response_1.CountryId)
         .Create();

        PersonAddRequest person_request_2 = _fixture.Build<PersonAddRequest>()
         .With(temp => temp.PersonName, "Mary")
         .With(temp => temp.Email, "someone_2@example.com")
         .With(temp => temp.CountryId, country_response_1.CountryId)
         .Create();

        PersonAddRequest person_request_3 = _fixture.Build<PersonAddRequest>()
         .With(temp => temp.PersonName, "Rahman")
         .With(temp => temp.Email, "someone_3@example.com")
         .With(temp => temp.CountryId, country_response_2.CountryId)
         .Create();


        List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };

        List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

        foreach (PersonAddRequest person_request in person_requests)
        {
            PersonResponse person_response = await _personService.AddPerson(person_request);
            person_response_list_from_add.Add(person_response);
        }

        _testOutputHelper.WriteLine("Expected:");
        foreach (PersonResponse person_response_from_add in person_response_list_from_add)
        {
            _testOutputHelper.WriteLine(person_response_from_add.ToString());
        }
        List<PersonResponse> allPersons = await _personService.GetAllPersons();

        List<PersonResponse> persons_list_from_sort = await _personService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

        _testOutputHelper.WriteLine("Actual:");
        foreach (PersonResponse person_response_from_get in persons_list_from_sort)
        {
            _testOutputHelper.WriteLine(person_response_from_get.ToString());
        }


        persons_list_from_sort.Should().BeInDescendingOrder(temp => temp.PersonName);
    }
    #endregion


    #region UpdatePerson

    [Fact]
    public async Task UpdatePerson_NullPerson()
    {
        PersonUpdateRequest? person_update_request = null;

        Func<Task> action = async () =>
        {
            await _personService.UpdatePerson(person_update_request);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();
    }


    [Fact]
    public async Task UpdatePerson_InvalidPersonID()
    {
        PersonUpdateRequest? person_update_request = _fixture.Build<PersonUpdateRequest>()
         .Create();

        Func<Task> action = async () =>
        {
            await _personService.UpdatePerson(person_update_request);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }


    [Fact]
    public async Task UpdatePerson_PersonNameIsNull()
    {
        CountryAddRequest country_request = _fixture.Create<CountryAddRequest>();

        CountryResponse country_response = await _countriesService.AddCountry(country_request);

        PersonAddRequest person_add_request = _fixture.Build<PersonAddRequest>()
         .With(temp => temp.PersonName, "Rahman")
         .With(temp => temp.Email, "someone@example.com")
         .With(temp => temp.CountryId, country_response.CountryId)
         .Create();


        PersonResponse person_response_from_add = await _personService.AddPerson(person_add_request);

        PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
        person_update_request.PersonName = null;


        var action = async () =>
        {
            await _personService.UpdatePerson(person_update_request);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }


    [Fact]
    public async Task UpdatePerson_PersonFullDetailsUpdation()
    {
        CountryAddRequest country_request = _fixture.Create<CountryAddRequest>();

        CountryResponse country_response = await _countriesService.AddCountry(country_request);

        PersonAddRequest person_add_request = _fixture.Build<PersonAddRequest>()
         .With(temp => temp.PersonName, "Rahman")
         .With(temp => temp.Email, "someone@example.com")
         .With(temp => temp.CountryId, country_response.CountryId)
         .Create();

        PersonResponse person_response_from_add = await _personService.AddPerson(person_add_request);

        PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
        person_update_request.PersonName = "William";
        person_update_request.Email = "william@example.com";

        PersonResponse person_response_from_update = await _personService.UpdatePerson(person_update_request);

        PersonResponse? person_response_from_get = await _personService.GetPersonByPersonId(person_response_from_update.PersonId);

        person_response_from_update.Should().Be(person_response_from_get);
    }

    #endregion


    #region DeletePerson

    [Fact]
    public async Task DeletePerson_ValidPersonID()
    {
        CountryAddRequest country_request = _fixture.Create<CountryAddRequest>();

        CountryResponse country_response = await _countriesService.AddCountry(country_request);

        PersonAddRequest person_add_request = _fixture.Build<PersonAddRequest>()
         .With(temp => temp.PersonName, "Rahman")
         .With(temp => temp.Email, "someone@example.com")
         .With(temp => temp.CountryId, country_response.CountryId)
         .Create();

        PersonResponse person_response_from_add = await _personService.AddPerson(person_add_request);


        bool isDeleted = await _personService.DeletePerson(person_response_from_add.PersonId);

        isDeleted.Should().BeTrue();
    }


    [Fact]
    public async Task DeletePerson_InvalidPersonID()
    {
        bool isDeleted = await _personService.DeletePerson(Guid.NewGuid());

        isDeleted.Should().BeFalse();
    }

    #endregion
}
