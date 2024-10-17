using Entities;
using ServiceContracts.DTO.Enums;

namespace ServiceContracts.DTO;

public class PersonResponse
{
    public Guid PersonId { get; set; }
    public string? PersonName { get; set; }
    public string? Email { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? Gender { get; set; }
    public Guid? CountryId { get; set; }

    public string? Country { get; set; }
    public string? Address { get; set; }

    public double? Age { get; set; }

    public bool ReceiveNewsLetters { get; set; }

    public override string ToString()
    {
        return $"PersonId: {PersonId}, " +
               $"PersonName: {PersonName ?? "N/A"}, " +
               $"Email: {Email ?? "N/A"}, " +
               $"DateOfBirth: {DateOfBirth?.ToString("yyyy-MM-dd") ?? "N/A"}, " +
               $"Gender: {Gender ?? "N/A"}, " +
               $"CountryId: {CountryId?.ToString() ?? "N/A"}, " +
               $"Country: {Country ?? "N/A"}, " +
               $"Address: {Address ?? "N/A"}, " +
               $"Age: {Age?.ToString() ?? "N/A"}, " +
               $"ReceiveNewsLetters: {ReceiveNewsLetters}";
    }

    public override bool Equals(object? obj)
    {
        if(obj is null)
            return false;

        if(obj.GetType() != typeof(PersonResponse)) return false;

        PersonResponse other = (PersonResponse)obj;

        return other.PersonId == PersonId && other.PersonName == PersonName && other.Email == Email && other.DateOfBirth == DateOfBirth &&
            other.Gender == Gender && other.CountryId == CountryId && other.Country == Country && other.Address == Address && other.Age == Age
            && other.ReceiveNewsLetters == ReceiveNewsLetters;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public PersonUpdateRequest ToPersonUpdateRequest()
    {
        return new PersonUpdateRequest() { PersonId = PersonId, PersonName = PersonName, Email = Email, DateOfBirth = DateOfBirth, Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions), Gender, true), Address = Address, CountryId = CountryId, ReceiveNewsLetters = ReceiveNewsLetters };
    }
}

public static class PersonExtensions
{
    public static PersonResponse ToPersonResponse(this Person person)
    {
        PersonResponse response = new PersonResponse()
        {
            PersonId = person.PersonId,
            PersonName = person.PersonName,
            Email = person.Email,
            DateOfBirth = person.DateOfBirth,
            Gender = person.Gender,
            Address = person.Address,
            ReceiveNewsLetters = person.ReceiveNewsLetters,
            CountryId = person.CountryId,
            Age= person.DateOfBirth!=null ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25) : null
        };

        return response;
    }
}