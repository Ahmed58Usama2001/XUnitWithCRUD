using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace CRUDTests;

public class CountriesServiceTest
{
    private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
    private readonly ICountriesRepository _countriesRepository;
    private readonly ICountriesService _countriesService;
    private readonly IFixture _fixture;

    public CountriesServiceTest()
    {
        _fixture = new Fixture();

        _countriesRepositoryMock = new Mock<ICountriesRepository>();
        _countriesRepository = _countriesRepositoryMock.Object;

         _countriesService = new CountriesService(_countriesRepository);
    }


    #region AddCountry

    [Fact]
    public async Task AddCountry_NullCountry_ToBeArgumentNullException()
    {
        CountryAddRequest? request = null;

        Country country = _fixture.Build<Country>()
            .With(c=>c.Persons, null as List<Person>)
            .Create();


        _countriesRepositoryMock
        .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
        .ReturnsAsync(country);


        var action = async () =>
        {
            await _countriesService.AddCountry(request);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();
    }


    [Fact]
    public async Task AddCountry_CountryNameIsNull_ToBeArgumentException()
    {
        CountryAddRequest? request = _fixture.Build<CountryAddRequest>()
         .With(temp => temp.CountryName, null as string)
         .Create();

        Country country = _fixture.Build<Country>()
            .With(temp=>temp.Persons, null as List<Person>)
            .Create();

        _countriesRepositoryMock.Setup(temp=>temp.AddCountry(It.IsAny<Country>()))
            .ReturnsAsync(country);

        var action = async () =>
        {
            await _countriesService.AddCountry(request);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }


    [Fact]
    public async Task AddCountry_DuplicateCountryName_ToBeArgumentException()
    {
        CountryAddRequest request1 = _fixture.Build<CountryAddRequest>()
            .With(temp=>temp.CountryName,"test")
         .Create();
        CountryAddRequest request2 = _fixture.Build<CountryAddRequest>()
          .With(temp => temp.CountryName, "test")
         .Create();

        Country first_country = request1.ToCountry();
        Country second_country = request2.ToCountry();

        _countriesRepositoryMock
            .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
            .ReturnsAsync(first_country);

        _countriesRepositoryMock
         .Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
         .ReturnsAsync(null as Country);

        await _countriesService.AddCountry(request1);


        var action = async () =>
        {
            _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(first_country);

            _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>())).ReturnsAsync(first_country);

            await _countriesService.AddCountry(request2);
        };

        await action.Should().ThrowAsync<ArgumentException>();
    }


    [Fact]
    public async Task AddCountry_FullCountry_ToBeSuccessful()
    {
        CountryAddRequest request = _fixture.Build<CountryAddRequest>()
         .Create();
        Country country = request.ToCountry();
        CountryResponse countryResponse = country.ToCountryResponse();

        _countriesRepositoryMock
         .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
         .ReturnsAsync(country);

        _countriesRepositoryMock
         .Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
         .ReturnsAsync(null as Country);

        CountryResponse countryFromAddCountry =  await _countriesService.AddCountry(request);

        country.CountryId= countryFromAddCountry.CountryId;
        countryResponse.CountryId = countryFromAddCountry.CountryId;


        countryFromAddCountry.CountryId.Should().NotBe(Guid.Empty);
        countryFromAddCountry.Should().BeEquivalentTo(countryResponse);
    }

    #endregion


    #region GetAllCountries

    [Fact]
    public async Task GetAllCountries_EmptyList()
    {
        List<Country> countriesEmptyList = new List<Country>();

        _countriesRepositoryMock.Setup(temp => temp.GetAllCountries())
            .ReturnsAsync(countriesEmptyList);

        List<CountryResponse> countryResponses =await _countriesService.GetAllCountries();

        countryResponses.Should().BeEmpty();
    }


    [Fact]
    public async Task GetAllCountries_ShouldHaveFewCountries()
    {
        List<Country> country_list = new List<Country>() {
        _fixture.Build<Country>()
        .With(temp => temp.Persons, null as List<Person>).Create(),
        _fixture.Build<Country>()
        .With(temp => temp.Persons, null as List<Person>).Create()
      };

        List<CountryResponse> countries = country_list.Select(c=>c.ToCountryResponse()).ToList();

        _countriesRepositoryMock.Setup(temp=>temp.GetAllCountries())
            .ReturnsAsync(country_list);

        List<CountryResponse> actualCountryResponseList = await _countriesService.GetAllCountries();

        actualCountryResponseList.Should().BeEquivalentTo(countries);
    }
    #endregion


    #region GetCountryByCountryID

    [Fact]
    public async Task GetCountryByCountryID_NullCountryID_ToBeNull()
    {
        Guid? countrID = null;

        _countriesRepositoryMock
 .Setup(temp => temp.GetCountryByCountryID(It.IsAny<Guid>()))
 .ReturnsAsync(null as Country);

        CountryResponse? country_response_from_get_method = await _countriesService.GetCountryByCountryId(countrID);

        country_response_from_get_method.Should().BeNull();
    }


    [Fact]
    public async Task GetCountryByCountryID_ValidCountryID_ToBeSuccessful()
    {
        Country? country = _fixture.Build<Country>()
            .With(temp=>temp.Persons , null as List<Person>)
         .Create();
        CountryResponse countryResponse = country.ToCountryResponse();

        _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryID(It.IsAny<Guid>()))
            .ReturnsAsync(country);

        CountryResponse? country_response_from_get = await _countriesService.GetCountryByCountryId(country.CountryId);

        country_response_from_get.Should().Be(countryResponse);
    }
    #endregion
}