using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace CRUDTests;

public class CountriesServiceTest
{   
    private readonly ICountriesService _countriesService;
    public CountriesServiceTest()
    {
        _countriesService = new CountriesService();
    }

    #region AddCountry
    [Fact]
    public void AddCountry_NullCountry()
    {
        CountryAddRequest request = null!;

        Assert.Throws<ArgumentNullException>(() =>
        {
            _countriesService.AddCountry(request);
        });
    }

    [Fact]
    public void AddCountry_NullCountryName()
    {
        CountryAddRequest request = new CountryAddRequest()
        {
            CountryName = null
        };

        Assert.Throws<ArgumentException>(() =>
        {
            _countriesService.AddCountry(request);
        });
    }

    [Fact]
    public void AddCountry_DuplicateCountryName()
    {
        CountryAddRequest request1 = new CountryAddRequest()
        {
            CountryName = "USA"
        };

        CountryAddRequest request2 = new CountryAddRequest()
        {
            CountryName = "USA"
        };

        Assert.Throws<ArgumentException>(() =>
        {
            _countriesService.AddCountry(request1);
            _countriesService.AddCountry(request2);

        });
    }

    [Fact]
    public void AddCountry_ProperCountryDetails()
    {
        CountryAddRequest request = new CountryAddRequest()
        { CountryName = "Japan" };

        CountryResponse response = _countriesService.AddCountry(request);
        List<CountryResponse> responseList = _countriesService.GetAllCountries();

        Assert.True(response.CountryId != Guid.Empty);
        Assert.Contains(response, responseList);
    }
    #endregion

    #region GetAllCountries
    [Fact]
    public void GetAllCountries_EmptyList()
    {
        List<CountryResponse> countries = _countriesService.GetAllCountries();

        Assert.Empty(countries);
    }

    [Fact]
    public void GetAllCountries_AddFewCountries()
    {
        List<CountryAddRequest> countryAddRequests = new List<CountryAddRequest>()
        {
            new CountryAddRequest {CountryName="USA"},
            new CountryAddRequest {CountryName="UK"}
        };

        List<CountryResponse> ExpectedAddedCountries = new List<CountryResponse>();
        foreach (var country in countryAddRequests)
        {
            ExpectedAddedCountries.Add(_countriesService.AddCountry(country));
        }

        List<CountryResponse> ActualCountries = _countriesService.GetAllCountries();

        foreach (var expectedCountry in ExpectedAddedCountries)
        {
            Assert.Contains(expectedCountry, ActualCountries);
        }
    }

    #endregion

    #region GetCountryByCountryId

    [Fact]
    public void GetCountryByCountryId_NullCountryId()
    {
        Guid? countryId = null;

        CountryResponse?countryResponse= _countriesService.GetCountryByCountryId(countryId);

        Assert.Null(countryResponse);
    }

    [Fact]
    public void GetCountryByCountryId_ValidCountryId()
    {
        CountryAddRequest countryAddRequest = new CountryAddRequest { CountryName = "China" };

        CountryResponse countryAddResponse = _countriesService.AddCountry(countryAddRequest);

        CountryResponse countryGetResponse = _countriesService.GetCountryByCountryId(countryAddResponse.CountryId);

        Assert.Equal(countryGetResponse, countryAddResponse);
    }
    #endregion
}
