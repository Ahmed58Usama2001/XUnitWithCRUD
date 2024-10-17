using Entities;

namespace ServiceContracts.DTO;

/// <summary>
/// DTO works as a response for most of Countries service methods
/// </summary>
public class CountryResponse
{
    public Guid CountryId { get; set; }
    public string? CountryName { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj == null) return false;

        if (obj.GetType() != typeof(CountryResponse))
            return false;

        CountryResponse other = (CountryResponse)obj;

        return this.CountryId==other.CountryId && this.CountryName==other.CountryName;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}


public static class CountryExtensions
{
    public static CountryResponse ToCountryResponse(this Country country)
    {
        CountryResponse response = new CountryResponse()
        {
            CountryId = country.CountryId,
            CountryName = country.CountryName,
        };

        return response;
    }
}
