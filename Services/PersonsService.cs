using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.Helpers;
using System;
using System.ComponentModel.DataAnnotations;

namespace Services;

public class PersonsService : IPersonService
{
    private readonly List<Person> _persons;
    private readonly ICountriesService _countryService;

    public PersonsService()
    {
        _countryService = new CountriesService();
        _persons = new List<Person>();
    }

    private PersonResponse ConvertPersonToPersonResponse(Person person)
    {
        PersonResponse personResponse = person.ToPersonResponse();
        personResponse.Country = _countryService.GetCountryByCountryId(personResponse.CountryId)?.CountryName;

        return personResponse;
    }

    public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
    {
        if (personAddRequest == null)
            throw new ArgumentNullException(nameof(personAddRequest));

        ValidationHelper.ModelValidation(personAddRequest);

        Person person = personAddRequest.ToPerson();
        person.PersonId=Guid.NewGuid();

        _persons.Add(person);
        
        return ConvertPersonToPersonResponse(person);
    }

    public List<PersonResponse> GetAllPersons()
    {
        throw new NotImplementedException();
    }

    public PersonResponse? GetPersonByPersonId(Guid? personId)
    {
        if(personId == null)
            return null;

        Person? person = _persons.FirstOrDefault(p => p.PersonId == personId);

        if (person == null)
            return null;

        return person.ToPersonResponse();
    }
}
