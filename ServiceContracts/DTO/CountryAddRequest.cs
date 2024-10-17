using Entities;

namespace ServiceContracts.DTO;

/// <summary>
/// Dto class for adding a new country
/// </summary>
public class CountryAddRequest
{
    public string? CountryName { get; set; }

    public Country ToCountry()
    {
        var country = new Country()
        {
            CountryName = CountryName
        };

        return country;
    }
}
