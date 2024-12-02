using Entities;
using Microsoft.EntityFrameworkCore;
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
    public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
    {
        if (countryAddRequest == null)
            throw new ArgumentNullException(nameof(countryAddRequest));

        if (countryAddRequest.CountryName == null)
            throw new ArgumentException(nameof(countryAddRequest.CountryName));
        

        if ( _dbContext.Countries.Where(c => c.CountryName == countryAddRequest.CountryName).Any())
            throw new ArgumentException("Given country name already exists");

        Country country = countryAddRequest.ToCountry();

        country.CountryId = Guid.NewGuid();

       await _dbContext.Countries.AddAsync(country);
        await _dbContext.SaveChangesAsync();

        return country.ToCountryResponse();
    }

    public async Task<List<CountryResponse>> GetAllCountries()
    {
        return await  _dbContext.Countries.Select(c => c.ToCountryResponse()).ToListAsync();
    }

    public async Task<CountryResponse?> GetCountryByCountryId(Guid? countryId)
    {
        if (countryId is null)
            return null;

        Country? country = await _dbContext.Countries.FirstOrDefaultAsync(c => c.CountryId == countryId);

        if (country == null)
            return null;

        return country.ToCountryResponse();
    }
}
