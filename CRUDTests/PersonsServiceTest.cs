using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.DTO.Enums;
using Services;

namespace CRUDTests;

public class PersonsServiceTest
{
    private readonly IPersonService _personService;
    private readonly ICountriesService _countriesService;

    public PersonsServiceTest()
    {
        _personService = new PersonsService();
        _countriesService = new CountriesService();
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
}
