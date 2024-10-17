using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.DTO.Enums;
using Services;
using Xunit.Abstractions;

namespace CRUDTests;

public class PersonsServiceTest
{
    private readonly IPersonService _personService;
    private readonly ICountriesService _countriesService;
    private readonly ITestOutputHelper _testOutputHelper;

    public PersonsServiceTest(ITestOutputHelper testOutputHelper)
    {
        _personService = new PersonsService();
        _countriesService = new CountriesService();
        _testOutputHelper = testOutputHelper;
    }

    #region AddPerson
    [Fact]
    public void AddPerson_NullPerson()
    {
        PersonAddRequest? personAddRequest = null;

       

        Assert.Throws<ArgumentNullException> (() =>
            {
                PersonResponse personResponse = _personService.AddPerson(personAddRequest);
            });
    }

    [Fact]
    public void AddPerson_NullPersonName()
    {
        PersonAddRequest? personAddRequest = new PersonAddRequest()
        {
            PersonName= null
        };


        Assert.Throws<ArgumentException>(() =>
        {
            PersonResponse personResponse = _personService.AddPerson(personAddRequest);
        });
    }

    [Fact]
    public void AddPerson_ProperPersonDetails()
    {
        PersonAddRequest? personAddRequest = new PersonAddRequest()
        {
            PersonName = "Ahmed" , Address="Tanta" , CountryId=Guid.NewGuid() , DateOfBirth=DateTime.Parse("2001-8-5"),
            Email="Ahmed@IEEE.com" , Gender=GenderOptions.Male , ReceiveNewsLetters=true
        };

        PersonResponse personResponse = _personService.AddPerson(personAddRequest);
        List<PersonResponse> personsList = _personService.GetAllPersons();

        Assert.True(personResponse.PersonId != Guid.Empty);
        Assert.Contains(personResponse , personsList);
    }
    #endregion

    #region GetPersonByPersonId
    [Fact]
    public void GetPersonByPersonId_NullPersonId()
    {
        Guid? personId = null;

        PersonResponse? personResponse= _personService.GetPersonByPersonId(personId);

        Assert.Null(personResponse);
    }

    [Fact]
    public void GetPersonByPersonId_WithPersonId()
    {
        CountryAddRequest countryAddRequest = new CountryAddRequest()
        {
            CountryName = "China"
        };
        CountryResponse countryResponse = _countriesService.AddCountry(countryAddRequest);

        PersonAddRequest personAddRequest = new PersonAddRequest()
        { Address="Tanta" , CountryId=countryResponse.CountryId, DateOfBirth=DateTime.Parse("2001-8-5") ,
            Email="Ahmed@gmail.com" , Gender=GenderOptions.Male, PersonName="Ahmed" , ReceiveNewsLetters=false };

        PersonResponse personAddResponse = _personService.AddPerson(personAddRequest) ;

        PersonResponse? personGetResponse=_personService.GetPersonByPersonId(personAddResponse.PersonId) ;

        Assert.Equal(personAddResponse, personGetResponse);
    }
    #endregion

    #region GetAllPersons
    [Fact]
    public void GetAllPersons_EmptyList()
    {
        List<PersonResponse> personResponse = _personService.GetAllPersons();

        Assert.Empty(personResponse);
    }

    [Fact]
    public void GetAllPersons_AddFewPersons()
    {
        CountryAddRequest countryAddRequest1 = new CountryAddRequest()
        {
            CountryName = "USA"
        };
        CountryAddRequest countryAddRequest2 = new CountryAddRequest()
        {
            CountryName = "UK"
        };

        CountryResponse countryResponse1= _countriesService.AddCountry(countryAddRequest1 );
        CountryResponse countryResponse2 = _countriesService.AddCountry(countryAddRequest2 );

        PersonAddRequest personAddRequest1 = new PersonAddRequest()
        {
            PersonName = "Ahmed",
            Address = "Tanta",
            CountryId = countryResponse1.CountryId,
            DateOfBirth = DateTime.Parse("2001-8-5"),
            Email = "Ahmed@IEEE.com",
            Gender = GenderOptions.Male,
            ReceiveNewsLetters = true
        }; 
        
        PersonAddRequest personAddRequest2 = new PersonAddRequest()
        {
            PersonName = "Mohamed",
            Address = "Tanta",
            CountryId = countryResponse1.CountryId,
            DateOfBirth = DateTime.Parse("2001-8-5"),
            Email = "Ahmed@IEEE.com",
            Gender = GenderOptions.Male,
            ReceiveNewsLetters = true
        }; 
        
        PersonAddRequest personAddRequest3 = new PersonAddRequest()
        {
            PersonName = "Omar",
            Address = "Tanta",
            CountryId = countryResponse2.CountryId,
            DateOfBirth = DateTime.Parse("2001-8-5"),
            Email = "Ahmed@IEEE.com",
            Gender = GenderOptions.Male,
            ReceiveNewsLetters = true
        };

        List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>()
        {
            personAddRequest1, personAddRequest2, personAddRequest3
        };

        

        List<PersonResponse> personResponses = new List<PersonResponse>();
        foreach (var addRequest in personAddRequests)
        {
            personResponses.Add(_personService.AddPerson(addRequest));
        }

        _testOutputHelper.WriteLine("Expected");
        foreach (var person in personResponses)
        {
            _testOutputHelper.WriteLine(person.ToString());
        }

        List<PersonResponse> personResponsesFromGet=_personService.GetAllPersons();

        _testOutputHelper.WriteLine("Actual");
        foreach (var person in personResponsesFromGet)
        {
            _testOutputHelper.WriteLine(person.ToString());
        }

        foreach (var personResponse in personResponses)
        {
            Assert.Contains(personResponse, personResponsesFromGet);
        }

    }
    #endregion

    #region GetFilteredPersons
    [Fact]
    public void GetFilteredPersons_EmptySearchText()
    {
        CountryAddRequest countryAddRequest1 = new CountryAddRequest()
        {
            CountryName = "USA"
        };
        CountryAddRequest countryAddRequest2 = new CountryAddRequest()
        {
            CountryName = "UK"
        };

        CountryResponse countryResponse1 = _countriesService.AddCountry(countryAddRequest1);
        CountryResponse countryResponse2 = _countriesService.AddCountry(countryAddRequest2);

        PersonAddRequest personAddRequest1 = new PersonAddRequest()
        {
            PersonName = "Ahmed",
            Address = "Tanta",
            CountryId = countryResponse1.CountryId,
            DateOfBirth = DateTime.Parse("2001-8-5"),
            Email = "Ahmed@IEEE.com",
            Gender = GenderOptions.Male,
            ReceiveNewsLetters = true
        };

        PersonAddRequest personAddRequest2 = new PersonAddRequest()
        {
            PersonName = "Mohamed",
            Address = "Tanta",
            CountryId = countryResponse1.CountryId,
            DateOfBirth = DateTime.Parse("2001-8-5"),
            Email = "Ahmed@IEEE.com",
            Gender = GenderOptions.Male,
            ReceiveNewsLetters = true
        };

        PersonAddRequest personAddRequest3 = new PersonAddRequest()
        {
            PersonName = "Omar",
            Address = "Tanta",
            CountryId = countryResponse2.CountryId,
            DateOfBirth = DateTime.Parse("2001-8-5"),
            Email = "Ahmed@IEEE.com",
            Gender = GenderOptions.Male,
            ReceiveNewsLetters = true
        };

        List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>()
        {
            personAddRequest1, personAddRequest2, personAddRequest3
        };



        List<PersonResponse> personResponses = new List<PersonResponse>();
        foreach (var addRequest in personAddRequests)
        {
            personResponses.Add(_personService.AddPerson(addRequest));
        }

        _testOutputHelper.WriteLine("Expected");
        foreach (var person in personResponses)
        {
            _testOutputHelper.WriteLine(person.ToString());
        }

        List<PersonResponse> personResponsesFromGet = _personService.GetFilteredPersons(nameof(PersonResponse.PersonName) , "");

        _testOutputHelper.WriteLine("Actual");
        foreach (var person in personResponsesFromGet)
        {
            _testOutputHelper.WriteLine(person.ToString());
        }

        foreach (var personResponse in personResponses)
        {
            Assert.Contains(personResponse, personResponsesFromGet);
        }

    }

    [Fact]
    public void GetFilteredPersons_SearchByPersonName()
    {
        CountryAddRequest countryAddRequest1 = new CountryAddRequest()
        {
            CountryName = "USA"
        };
        CountryAddRequest countryAddRequest2 = new CountryAddRequest()
        {
            CountryName = "UK"
        };

        CountryResponse countryResponse1 = _countriesService.AddCountry(countryAddRequest1);
        CountryResponse countryResponse2 = _countriesService.AddCountry(countryAddRequest2);

        PersonAddRequest personAddRequest1 = new PersonAddRequest()
        {
            PersonName = "Ahmed",
            Address = "Tanta",
            CountryId = countryResponse1.CountryId,
            DateOfBirth = DateTime.Parse("2001-8-5"),
            Email = "Ahmed@IEEE.com",
            Gender = GenderOptions.Male,
            ReceiveNewsLetters = true
        };

        PersonAddRequest personAddRequest2 = new PersonAddRequest()
        {
            PersonName = "Mohamed",
            Address = "Tanta",
            CountryId = countryResponse1.CountryId,
            DateOfBirth = DateTime.Parse("2001-8-5"),
            Email = "Ahmed@IEEE.com",
            Gender = GenderOptions.Male,
            ReceiveNewsLetters = true
        };

        PersonAddRequest personAddRequest3 = new PersonAddRequest()
        {
            PersonName = "Omar",
            Address = "Tanta",
            CountryId = countryResponse2.CountryId,
            DateOfBirth = DateTime.Parse("2001-8-5"),
            Email = "Ahmed@IEEE.com",
            Gender = GenderOptions.Male,
            ReceiveNewsLetters = true
        };

        List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>()
        {
            personAddRequest1, personAddRequest2, personAddRequest3
        };



        List<PersonResponse> personResponses = new List<PersonResponse>();
        foreach (var addRequest in personAddRequests)
        {
            personResponses.Add(_personService.AddPerson(addRequest));
        }

        _testOutputHelper.WriteLine("Expected");
        foreach (var person in personResponses)
        {
            _testOutputHelper.WriteLine(person.ToString());
        }

        List<PersonResponse> personResponsesFromGet = _personService.GetFilteredPersons(nameof(PersonResponse.PersonName), "o");

        _testOutputHelper.WriteLine("Actual");
        foreach (var person in personResponsesFromGet)
        {
            _testOutputHelper.WriteLine(person.ToString());
        }

        foreach (var personResponse in personResponses)
        {
            if(personResponse.PersonName != null)
            {
                if (personResponse.PersonName.Contains("o", StringComparison.OrdinalIgnoreCase))
                    Assert.Contains(personResponse, personResponsesFromGet);
            }
        }

    }
    #endregion

    #region GetSortedPersons 
    [Fact]
    public void GetSortedPersons_SearchByPersonName()
    {
        CountryAddRequest countryAddRequest1 = new CountryAddRequest()
        {
            CountryName = "USA"
        };
        CountryAddRequest countryAddRequest2 = new CountryAddRequest()
        {
            CountryName = "UK"
        };

        CountryResponse countryResponse1 = _countriesService.AddCountry(countryAddRequest1);
        CountryResponse countryResponse2 = _countriesService.AddCountry(countryAddRequest2);

        PersonAddRequest personAddRequest1 = new PersonAddRequest()
        {
            PersonName = "Ahmed",
            Address = "Tanta",
            CountryId = countryResponse1.CountryId,
            DateOfBirth = DateTime.Parse("2001-8-5"),
            Email = "Ahmed@IEEE.com",
            Gender = GenderOptions.Male,
            ReceiveNewsLetters = true
        };

        PersonAddRequest personAddRequest2 = new PersonAddRequest()
        {
            PersonName = "Mohamed",
            Address = "Tanta",
            CountryId = countryResponse1.CountryId,
            DateOfBirth = DateTime.Parse("2001-8-5"),
            Email = "Ahmed@IEEE.com",
            Gender = GenderOptions.Male,
            ReceiveNewsLetters = true
        };

        PersonAddRequest personAddRequest3 = new PersonAddRequest()
        {
            PersonName = "Omar",
            Address = "Tanta",
            CountryId = countryResponse2.CountryId,
            DateOfBirth = DateTime.Parse("2001-8-5"),
            Email = "Ahmed@IEEE.com",
            Gender = GenderOptions.Male,
            ReceiveNewsLetters = true
        };

        List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>()
        {
            personAddRequest1, personAddRequest2, personAddRequest3
        };



        List<PersonResponse> personResponses = new List<PersonResponse>();
        foreach (var addRequest in personAddRequests)
        {
            personResponses.Add(_personService.AddPerson(addRequest));
        }

        _testOutputHelper.WriteLine("Expected");
        foreach (var person in personResponses)
        {
            _testOutputHelper.WriteLine(person.ToString());
        }

        List<PersonResponse> allPersons= _personService.GetAllPersons();
        List<PersonResponse> personResponsesFromGet = _personService.GetSortedPersons(allPersons, nameof(PersonResponse.PersonName),
           SortOrderOptions.DESC);

        _testOutputHelper.WriteLine("Actual");
        foreach (var person in personResponsesFromGet)
        {
            _testOutputHelper.WriteLine(person.ToString());
        }

        personResponses = personResponses.OrderByDescending(p => p.PersonName).ToList();

        for (int i = 0; i < personResponses.Count; i++)
        {
            Assert.Equal(personResponses[i], personResponsesFromGet[i]);
        }

    }
    #endregion

    #region UpdatePerson

    [Fact]
    public void UpdatePerson_NullPerson()
    {
        PersonUpdateRequest? person_update_request = null;

        Assert.Throws<ArgumentNullException>(() => {
            _personService.UpdatePerson(person_update_request);
        });
    }


    [Fact]
    public void UpdatePerson_InvalidPersonID()
    {
        PersonUpdateRequest? person_update_request = new PersonUpdateRequest() { PersonId = Guid.NewGuid() };

        Assert.Throws<ArgumentException>(() => {
            _personService.UpdatePerson(person_update_request);
        });
    }


    [Fact]
    public void UpdatePerson_PersonNameIsNull()
    {
        CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "UK" };
        CountryResponse country_response_from_add = _countriesService.AddCountry(country_add_request);

        PersonAddRequest person_add_request = new PersonAddRequest() { PersonName = "John", CountryId = country_response_from_add.CountryId, Email = "john@example.com", Address = "address...", Gender = GenderOptions.Male };
        PersonResponse person_response_from_add = _personService.AddPerson(person_add_request);

        PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
        person_update_request.PersonName = null;


        Assert.Throws<ArgumentException>(() => {
            _personService.UpdatePerson(person_update_request);
        });

    }


    [Fact]
    public void UpdatePerson_PersonFullDetailsUpdation()
    {
        CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "UK" };
        CountryResponse country_response_from_add = _countriesService.AddCountry(country_add_request);

        PersonAddRequest person_add_request = new PersonAddRequest() { PersonName = "John", CountryId = country_response_from_add.CountryId, Address = "Abc road", DateOfBirth = DateTime.Parse("2000-01-01"), Email = "abc@example.com", Gender = GenderOptions.Male, ReceiveNewsLetters = true };

        PersonResponse person_response_from_add = _personService.AddPerson(person_add_request);

        PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
        person_update_request.PersonName = "William";
        person_update_request.Email = "william@example.com";

        PersonResponse person_response_from_update = _personService.UpdatePerson(person_update_request);

        PersonResponse? person_response_from_get = _personService.GetPersonByPersonId(person_response_from_update.PersonId);

        
        Assert.Equal(person_response_from_get, person_response_from_update);

    }

    #endregion

    #region DeletePerson

    [Fact]
    public void DeletePerson_ValidPersonID()
    {
        CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "USA" };
        CountryResponse country_response_from_add = _countriesService.AddCountry(country_add_request);

        PersonAddRequest person_add_request = new PersonAddRequest() { PersonName = "Jones", Address = "address", CountryId = country_response_from_add.CountryId, DateOfBirth = Convert.ToDateTime("2010-01-01"), Email = "jones@example.com", Gender = GenderOptions.Male, ReceiveNewsLetters = true };

        PersonResponse person_response_from_add = _personService.AddPerson(person_add_request);


        bool isDeleted = _personService.DeletePerson(person_response_from_add.PersonId);

        Assert.True(isDeleted);
    }


    [Fact]
    public void DeletePerson_InvalidPersonID()
    {
        bool isDeleted = _personService.DeletePerson(Guid.NewGuid());

        Assert.False(isDeleted);
    }

    #endregion
}
