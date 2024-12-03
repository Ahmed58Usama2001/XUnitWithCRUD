using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace CRUDTests;

public class CountriesServiceTest
{   
    private readonly ICountriesService _countriesService;
    public CountriesServiceTest()
    {
        _countriesService = new CountriesService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));
    }

    #region AddCountry
    [Fact]
    public async Task AddCountry_NullCountry()
    {
        CountryAddRequest request = null!;

        await Assert.ThrowsAsync<ArgumentNullException>( async() =>
        {
          await  _countriesService.AddCountry(request);
        });
    }

    [Fact]
    public async Task AddCountry_NullCountryName()
    {
        CountryAddRequest request = new CountryAddRequest()
        {
            CountryName = null
        };

       await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
          await  _countriesService.AddCountry(request);
        });
    }

    [Fact]
    public async Task AddCountry_DuplicateCountryName()
    {
        CountryAddRequest request1 = new CountryAddRequest()
        {
            CountryName = "USA"
        };

        CountryAddRequest request2 = new CountryAddRequest()
        {
            CountryName = "USA"
        };

       await Assert.ThrowsAsync<ArgumentException>( async() =>
        {
            await _countriesService.AddCountry(request1);
            await _countriesService.AddCountry(request2);

        });
    }

    [Fact]
    public async Task AddCountry_ProperCountryDetails()
    {
        CountryAddRequest request = new CountryAddRequest()
        { CountryName = "Japan" };

        CountryResponse response =await _countriesService.AddCountry(request);
        List<CountryResponse> responseList =await _countriesService.GetAllCountries();

        Assert.True(response.CountryId != Guid.Empty);
        Assert.Contains(response, responseList);
    }
    #endregion

    #region GetAllCountries
    [Fact]
    public async Task GetAllCountries_EmptyList()
    {
        List<CountryResponse> countries =await _countriesService.GetAllCountries();

        Assert.Empty(countries);
    }

    [Fact]
    public async Task GetAllCountries_AddFewCountries()
    {
        List<CountryAddRequest> countryAddRequests = new List<CountryAddRequest>()
        {
            new CountryAddRequest {CountryName="USA"},
            new CountryAddRequest {CountryName="UK"}
        };

        List<CountryResponse> ExpectedAddedCountries = new List<CountryResponse>();
        foreach (var country in countryAddRequests)
        {
            ExpectedAddedCountries.Add(await _countriesService.AddCountry(country));
        }

        List<CountryResponse> ActualCountries =await _countriesService.GetAllCountries();

        foreach (var expectedCountry in ExpectedAddedCountries)
        {
            Assert.Contains(expectedCountry, ActualCountries);
        }
    }

    #endregion

    #region GetCountryByCountryId

    [Fact]
    public async Task GetCountryByCountryId_NullCountryId()
    {
        Guid? countryId = null;

        CountryResponse?countryResponse=await _countriesService.GetCountryByCountryId(countryId);

        Assert.Null(countryResponse);
    }

    [Fact]
    public async Task GetCountryByCountryId_ValidCountryId()
    {
        CountryAddRequest countryAddRequest = new CountryAddRequest { CountryName = "China" };

        CountryResponse countryAddResponse =await _countriesService.AddCountry(countryAddRequest);

        CountryResponse? countryGetResponse =await _countriesService.GetCountryByCountryId(countryAddResponse.CountryId);

        Assert.Equal(countryGetResponse, countryAddResponse);
    }
    #endregion
}
