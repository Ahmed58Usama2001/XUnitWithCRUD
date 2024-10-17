using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services;

public class CountriesService : ICountriesService
{
    private readonly List<Country> _countries;
    public CountriesService()
    {
        _countries = new List<Country>();
    }
    public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
    {
        if (countryAddRequest == null)
            throw new ArgumentNullException(nameof(countryAddRequest));

        if (countryAddRequest.CountryName == null)
            throw new ArgumentException(nameof(countryAddRequest.CountryName));
        

        if (_countries.Where(c => c.CountryName == countryAddRequest.CountryName).Any())
            throw new ArgumentException("Given country name already exists");

        Country country = countryAddRequest.ToCountry();

        country.CountryId = Guid.NewGuid();

        _countries.Add(country);

        return country.ToCountryResponse();
    }

    public List<CountryResponse> GetAllCountries()
    {
        return _countries.Select(c => c.ToCountryResponse()).ToList();
    }

    public CountryResponse? GetCountryByCountryId(Guid? countryId)
    {
        if (countryId is null)
            return null;

        Country? country = _countries.FirstOrDefault(c => c.CountryId == countryId);

        if (country == null)
            return null;

        return country.ToCountryResponse();
    }
}
