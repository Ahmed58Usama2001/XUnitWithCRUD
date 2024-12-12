using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace CRUDTests;

public class CountriesServiceTest
{
    private readonly ICountriesService _countriesService;
    private readonly IFixture _fixture;

    public CountriesServiceTest()
    {
        _fixture = new Fixture();

        var countriesInitialData = new List<Country>() { };

        DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
          new DbContextOptionsBuilder<ApplicationDbContext>().Options
         );

        ApplicationDbContext dbContext = dbContextMock.Object;
        dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);

        _countriesService = new CountriesService(null);
    }


    #region AddCountry

    [Fact]
    public async Task AddCountry_NullCountry()
    {
        CountryAddRequest? request = null;

        var action = async () =>
        {
            await _countriesService.AddCountry(request);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();
    }


    [Fact]
    public async Task AddCountry_CountryNameIsNull()
    {
        CountryAddRequest? request = _fixture.Build<CountryAddRequest>()
         .With(temp => temp.CountryName, null as string)
         .Create();

        var action = async () =>
        {
            await _countriesService.AddCountry(request);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }


    [Fact]
    public async Task AddCountry_DuplicateCountryName()
    {
        CountryAddRequest? request1 = _fixture.Build<CountryAddRequest>()
         .Create();
        CountryAddRequest? request2 = _fixture.Build<CountryAddRequest>()
         .With(temp => temp.CountryName, request1.CountryName)
         .Create();

        var action = async () =>
        {
            await _countriesService.AddCountry(request1);
            await _countriesService.AddCountry(request2);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }


    [Fact]
    public async Task AddCountry_ProperCountryDetails()
    {
        CountryAddRequest? request = _fixture.Build<CountryAddRequest>()
         .Create();

        CountryResponse response = await _countriesService.AddCountry(request);
        List<CountryResponse> countries_from_GetAllCountries = await _countriesService.GetAllCountries();

        response.CountryId.Should().NotBe(Guid.Empty);
        countries_from_GetAllCountries.Should().Contain(response);
    }

    #endregion


    #region GetAllCountries

    [Fact]
    public async Task GetAllCountries_EmptyList()
    {
        List<CountryResponse> actual_country_response_list = await _countriesService.GetAllCountries();

        actual_country_response_list.Should().BeEmpty();
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

        actualCountryResponseList.Should().BeEquivalentTo(countries_list_from_add_country);
    }
    #endregion


    #region GetCountryByCountryID

    [Fact]
    public async Task GetCountryByCountryID_NullCountryID()
    {
        Guid? countrID = null;

        CountryResponse? country_response_from_get_method = await _countriesService.GetCountryByCountryId(countrID);

        country_response_from_get_method.Should().BeNull();
    }


    [Fact]
    public async Task GetCountryByCountryID_ValidCountryID()
    {
        CountryAddRequest? country_add_request = _fixture.Build<CountryAddRequest>()
         .Create();

        CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

        CountryResponse? country_response_from_get = await _countriesService.GetCountryByCountryId(country_response_from_add.CountryId);

        country_response_from_get.Should().Be(country_response_from_add);
    }
    #endregion
}