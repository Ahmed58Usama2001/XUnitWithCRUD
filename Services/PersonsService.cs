using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.DTO.Enums;
using Services.Helpers;
using System;

namespace Services;

public class PersonsService : IPersonService
{
    private readonly ICountriesService _countryService;
    private readonly PersonsDbContext _dbContext;

    public PersonsService(ICountriesService countriesService ,
        PersonsDbContext dbContext)
    {
        _countryService = countriesService;
        _dbContext = dbContext;
    }

 

    public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
    {
        if (personAddRequest == null)
            throw new ArgumentNullException(nameof(personAddRequest));

        ValidationHelper.ModelValidation(personAddRequest);

        Person person = personAddRequest.ToPerson();
        person.PersonId=Guid.NewGuid();

       await  _dbContext.Persons.AddAsync(person);
        await _dbContext.SaveChangesAsync();

        // _dbContext.sp_InsertPerson(person);

        return person.ToPersonResponse();
    }

    public async Task<List<PersonResponse>> GetAllPersons()
    {
        var persons =await _dbContext.Persons.Include(nameof(Person.Country)).ToListAsync();

        return persons.Select(p => p.ToPersonResponse()).ToList();

        //return _dbContext.sp_GetAllPersons().Select(p=>p.ToPersonResponse()).ToList();
    }

    public async Task<PersonResponse?> GetPersonByPersonId(Guid? personId)
    {
        if(personId == null)
            return null;

        Person? person = await _dbContext.Persons.Include(nameof(Person.Country)).FirstOrDefaultAsync(p => p.PersonId == personId);

        if (person == null)
            return null;

        return person.ToPersonResponse();
    }

    public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
    {
        List<PersonResponse> allPersons = await GetAllPersons();
        List<PersonResponse> matchingPersons = allPersons;

        if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
            return matchingPersons;

        switch (searchBy)
        {
            case nameof(PersonResponse.PersonName):
                matchingPersons = allPersons.Where(temp =>
                (!string.IsNullOrEmpty(temp.PersonName) ?
                temp.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                break;

            case nameof(PersonResponse.Email):
                matchingPersons = allPersons.Where(temp =>
                (!string.IsNullOrEmpty(temp.Email) ?
                temp.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                break;


            case nameof(PersonResponse.DateOfBirth):
                matchingPersons = allPersons.Where(temp =>
                (temp.DateOfBirth != null) ?
                temp.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                break;

            case nameof(PersonResponse.Gender):
                matchingPersons = allPersons.Where(temp =>
                (!string.IsNullOrEmpty(temp.Gender) ?
                temp.Gender.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                break;

            case nameof(PersonResponse.CountryId):
                matchingPersons = allPersons.Where(temp =>
                (!string.IsNullOrEmpty(temp.Country) ?
                temp.Country.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                break;

            case nameof(PersonResponse.Address):
                matchingPersons = allPersons.Where(temp =>
                (!string.IsNullOrEmpty(temp.Address) ?
                temp.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                break;

            default: matchingPersons = allPersons; break;
        }
        return matchingPersons;
    }

    public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
    {
        if (string.IsNullOrEmpty(sortBy))
            return allPersons;

        List<PersonResponse> sortedPersons = (sortBy, sortOrder) switch
        {
            (nameof(PersonResponse.PersonName), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.Email), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.Email), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.DateOfBirth).ToList(),

            (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.DateOfBirth).ToList(),

            (nameof(PersonResponse.Age), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Age).ToList(),

            (nameof(PersonResponse.Age), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Age).ToList(),

            (nameof(PersonResponse.Gender), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.Gender), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.Country), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.Country), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.Address), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.Address), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.ReceiveNewsLetters).ToList(),

            (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.ReceiveNewsLetters).ToList(),

            _ => allPersons
        };

        return sortedPersons;
    }

    public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
    {
        if (personUpdateRequest == null)
            throw new ArgumentNullException(nameof(Person));

        ValidationHelper.ModelValidation(personUpdateRequest);

        Person? matchingPerson = _dbContext.Persons.FirstOrDefault(temp => temp.PersonId == personUpdateRequest.PersonId);
        if (matchingPerson == null)
        {
            throw new ArgumentException("Given person id doesn't exist");
        }

        matchingPerson.PersonName = personUpdateRequest.PersonName;
        matchingPerson.Email = personUpdateRequest.Email;
        matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
        matchingPerson.Gender = personUpdateRequest.Gender.ToString();
        matchingPerson.CountryId = personUpdateRequest.CountryId;
        matchingPerson.Address = personUpdateRequest.Address;
        matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

         await _dbContext.SaveChangesAsync();

        return matchingPerson.ToPersonResponse();
    }

    public async Task<bool> DeletePerson(Guid? personId)
    {
        if (personId == null)
        {
            throw new ArgumentNullException(nameof(personId));
        }

        Person? person =await _dbContext.Persons.FirstOrDefaultAsync(temp => temp.PersonId == personId);
        if (person == null)
            return false;

         _dbContext.Persons.Remove(_dbContext.Persons.First(temp => temp.PersonId == personId));
        await _dbContext.SaveChangesAsync();

        return true;
    }
}

