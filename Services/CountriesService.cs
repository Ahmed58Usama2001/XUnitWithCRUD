﻿using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services;

public class CountriesService : ICountriesService
{
    private readonly PersonsDbContext _dbContext;

    public CountriesService(PersonsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
    {
        if (countryAddRequest == null)
            throw new ArgumentNullException(nameof(countryAddRequest));

        if (countryAddRequest.CountryName == null)
            throw new ArgumentException(nameof(countryAddRequest.CountryName));
        

        if (_dbContext.Countries.Where(c => c.CountryName == countryAddRequest.CountryName).Any())
            throw new ArgumentException("Given country name already exists");

        Country country = countryAddRequest.ToCountry();

        country.CountryId = Guid.NewGuid();

        _dbContext.Countries.Add(country);
        _dbContext.SaveChanges();

        return country.ToCountryResponse();
    }

    public List<CountryResponse> GetAllCountries()
    {
        return _dbContext.Countries.Select(c => c.ToCountryResponse()).ToList();
    }

    public CountryResponse? GetCountryByCountryId(Guid? countryId)
    {
        if (countryId is null)
            return null;

        Country? country = _dbContext.Countries.FirstOrDefault(c => c.CountryId == countryId);

        if (country == null)
            return null;

        return country.ToCountryResponse();
    }
}
