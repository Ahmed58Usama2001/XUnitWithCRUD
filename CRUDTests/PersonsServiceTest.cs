using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.DTO.Enums;
using Services;
using Xunit.Abstractions;

namespace CRUDTests;

public class PersonsServiceTest
{
    private readonly IPersonsService _personService;
    private readonly ICountriesService _countriesService;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IFixture _fixture;

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

        _personService = new PersonsService(_countriesService,dbContext );

        _testOutputHelper = testOutputHelper;
    }

    #region AddPerson

    [Fact]
    public async Task AddPerson_NullPerson()
    {
        PersonAddRequest? personAddRequest = null;

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await _personService.AddPerson(personAddRequest);
        });
    }


    [Fact]
    public async Task AddPerson_PersonNameIsNull()
    {
        PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
         .With(temp => temp.PersonName, null as string)
         .Create();

        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _personService.AddPerson(personAddRequest);
        });
    }

    [Fact]
    public async Task AddPerson_ProperPersonDetails()
    {
        PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
         .With(temp => temp.Email, "someone@example.com")
         .Create();

        
        PersonResponse person_response_from_add = await _personService.AddPerson(personAddRequest);

        List<PersonResponse> persons_list = await _personService.GetAllPersons();

        
        Assert.True(person_response_from_add.PersonId != Guid.Empty);

        Assert.Contains(person_response_from_add, persons_list);
    }

    #endregion


    #region GetPersonByPersonID

    [Fact]
    public async Task GetPersonByPersonID_NullPersonID()
    {
        Guid? personID = null;

        PersonResponse? person_response_from_get = await _personService.GetPersonByPersonId(personID);

        Assert.Null(person_response_from_get);
    }


    [Fact]
    public async Task GetPersonByPersonID_WithPersonID()
    {
        //Arange
        CountryAddRequest country_request = _fixture.Create<CountryAddRequest>();

        CountryResponse country_response = await _countriesService.AddCountry(country_request);

        PersonAddRequest person_request = _fixture.Build<PersonAddRequest>()
         .With(temp => temp.Email, "email@sample.com")
         .Create();

        PersonResponse person_response_from_add = await _personService.AddPerson(person_request);

        PersonResponse? person_response_from_get = await _personService.GetPersonByPersonId(person_response_from_add.PersonId);

        Assert.Equal(person_response_from_add, person_response_from_get);
    }

    #endregion


    #region GetAllPersons

    [Fact]
    public async Task GetAllPersons_EmptyList()
    {
        List<PersonResponse> persons_from_get = await _personService.GetAllPersons();

        Assert.Empty(persons_from_get);
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
         .Create();

        PersonAddRequest person_request_2 = _fixture.Build<PersonAddRequest>()
         .With(temp => temp.Email, "someone_2@example.com")
         .Create();

        PersonAddRequest person_request_3 = _fixture.Build<PersonAddRequest>()
         .With(temp => temp.Email, "someone_3@example.com")
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

        foreach (PersonResponse person_response_from_add in person_response_list_from_add)
        {
            Assert.Contains(person_response_from_add, persons_list_from_get);
        }
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
         .Create();

        PersonAddRequest person_request_2 = _fixture.Build<PersonAddRequest>()
         .With(temp => temp.Email, "someone_2@example.com")
         .Create();

        PersonAddRequest person_request_3 = _fixture.Build<PersonAddRequest>()
         .With(temp => temp.Email, "someone_3@example.com")
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

        foreach (PersonResponse person_response_from_add in person_response_list_from_add)
        {
            Assert.Contains(person_response_from_add, persons_list_from_search);
        }
    }


    [Fact]
    public async Task GetFilteredPersons_SearchByPersonName()
    {
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

        foreach (PersonResponse person_response_from_add in person_response_list_from_add)
        {
            if (person_response_from_add.PersonName != null)
            {
                if (person_response_from_add.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase))
                {
                    Assert.Contains(person_response_from_add, persons_list_from_search);
                }
            }
        }
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
        person_response_list_from_add = person_response_list_from_add.OrderByDescending(temp => temp.PersonName).ToList();

        for (int i = 0; i < person_response_list_from_add.Count; i++)
        {
            Assert.Equal(person_response_list_from_add[i], persons_list_from_sort[i]);
        }
    }
    #endregion


    #region UpdatePerson

    [Fact]
    public async Task UpdatePerson_NullPerson()
    {
        PersonUpdateRequest? person_update_request = null;

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await _personService.UpdatePerson(person_update_request);
        });
    }


    [Fact]
    public async Task UpdatePerson_InvalidPersonID()
    {
       
        PersonUpdateRequest? person_update_request = _fixture.Build<PersonUpdateRequest>()
         .Create();

      
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            
            await _personService.UpdatePerson(person_update_request);
        });
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


     
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
           
            await _personService.UpdatePerson(person_update_request);
        });

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

        
        Assert.Equal(person_response_from_get, person_response_from_update);

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

        Assert.True(isDeleted);
    }


    [Fact]
    public async Task DeletePerson_InvalidPersonID()
    {
        bool isDeleted = await _personService.DeletePerson(Guid.NewGuid());

        Assert.False(isDeleted);
    }

    #endregion
}
