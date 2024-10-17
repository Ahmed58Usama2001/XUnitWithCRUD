using ServiceContracts.DTO;

namespace ServiceContracts;
/// <summary>
/// Represent business logic for Manipulating country entity
/// </summary>
public interface ICountriesService
{
    /// <summary>
    /// adds a country object to list of countries
    /// </summary>
    /// <param name="countryAddRequest">Country object to be added</param>
    /// <returns>Returns the country object after adding it (including the generated Id)</returns>
    CountryResponse AddCountry(CountryAddRequest? countryAddRequest);

    /// <summary>
    /// Get list of all countries form the countries list
    /// </summary>
    /// <returns>
    /// get list of all countries from countries list in form of CountryResponse</returns>
    List<CountryResponse> GetAllCountries();

    /// <summary>
    /// Get a specific country by the country id
    /// </summary>
    /// <param name="countryId"> the guid of the country we search for</param>
    /// <returns> a country in the shape of CountryResponse</returns>
    CountryResponse? GetCountryByCountryId(Guid? countryId);
}
