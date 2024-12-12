using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace CRUDTests;

public class CountriesServiceTest
{
    private readonly ICountriesService _countriesService;
    private readonly IFixture _fixture;

    //constructor
    public CountriesServiceTest()
    {
        _fixture = new Fixture();

        var countriesInitialData = new List<Country>() { };

        DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
          new DbContextOptionsBuilder<ApplicationDbContext>().Options
         );

        ApplicationDbContext dbContext = dbContextMock.Object;
        dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);

        _countriesService = new CountriesService(dbContext);
    }


    #region AddCountry

    [Fact]
    public async Task AddCountry_NullCountry()
    {
        CountryAddRequest? request = null;

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await _countriesService.AddCountry(request);
        });
    }

    [Fact]
    public async Task AddCountry_CountryNameIsNull()
    {
        CountryAddRequest? request = _fixture.Build<CountryAddRequest>()
         .With(temp => temp.CountryName, null as string)
         .Create();

        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _countriesService.AddCountry(request);
        });
    }


    [Fact]
    public async Task AddCountry_DuplicateCountryName()
    {
        CountryAddRequest? request1 = _fixture.Build<CountryAddRequest>()
         .Create();
        CountryAddRequest? request2 = _fixture.Build<CountryAddRequest>()
         .With(temp => temp.CountryName, request1.CountryName)
         .Create();

        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _countriesService.AddCountry(request1);
            await _countriesService.AddCountry(request2);
        });
    }


    [Fact]
    public async Task AddCountry_ProperCountryDetails()
    {
        CountryAddRequest? request = _fixture.Build<CountryAddRequest>()
         .Create();

        CountryResponse response = await _countriesService.AddCountry(request);
        List<CountryResponse> countries_from_GetAllCountries = await _countriesService.GetAllCountries();

        Assert.True(response.CountryId != Guid.Empty);
        Assert.Contains(response, countries_from_GetAllCountries);
    }

    #endregion


    #region GetAllCountries

    [Fact]
    public async Task GetAllCountries_EmptyList()
    {
        List<CountryResponse> actual_country_response_list = await _countriesService.GetAllCountries();

        Assert.Empty(actual_country_response_list);
    }


    [Fact]
    public async Task GetAllCountries_AddFewCountries()
    {
        List<CountryAddRequest> country_request_list = new List<CountryAddRequest>() {
      _fixture.Build<CountryAddRequest>()
       .Create(),
      _fixture.Build<CountryAddRequest>()
       .Create()
      };

        List<CountryResponse> countries_list_from_add_country = new List<CountryResponse>();

        foreach (CountryAddRequest country_request in country_request_list)
        {
            countries_list_from_add_country.Add(await _countriesService.AddCountry(country_request));
        }

        List<CountryResponse> actualCountryResponseList = await _countriesService.GetAllCountries();

        foreach (CountryResponse expected_country in countries_list_from_add_country)
        {
            Assert.Contains(expected_country, actualCountryResponseList);
        }
    }
    #endregion


    #region GetCountryByCountryID

    [Fact]
    public async Task GetCountryByCountryID_NullCountryID()
    {
        Guid? countrID = null;

        
        CountryResponse? country_response_from_get_method = await _countriesService.GetCountryByCountryId(countrID);


        Assert.Null(country_response_from_get_method);
    }


    [Fact]
    public async Task GetCountryByCountryID_ValidCountryID()
    {
        
        CountryAddRequest? country_add_request = _fixture.Build<CountryAddRequest>()
         .Create();

        CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

        CountryResponse? country_response_from_get = await _countriesService.GetCountryByCountryId(country_response_from_add.CountryId);

        Assert.Equal(country_response_from_add, country_response_from_get);
    }
    #endregion
}