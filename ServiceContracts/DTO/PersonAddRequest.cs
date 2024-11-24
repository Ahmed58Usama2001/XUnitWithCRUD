using Entities;
using ServiceContracts.DTO.Enums;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO;

public class PersonAddRequest
{
    [Required(ErrorMessage ="Person name is required")]
    public string? PersonName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage ="Enter a valid email")]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    public GenderOptions? Gender { get; set; }
    public Guid? CountryId { get; set; }

    public string? Address { get; set; }

    public bool ReceiveNewsLetters { get; set; }

    public Person ToPerson()
    {
        return new Person()
        {
            PersonName = PersonName,
            Email = Email,
            DateOfBirth = DateOfBirth,
            Gender = Gender.ToString(),
            CountryId = CountryId,
            Address = Address,
            ReceiveNewsLetters = ReceiveNewsLetters
        };
    }
}
