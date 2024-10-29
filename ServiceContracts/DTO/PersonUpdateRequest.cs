﻿using Entities;
using ServiceContracts.DTO.Enums;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO;

public class PersonUpdateRequest
{
    [Required(ErrorMessage = "Person Id is required")]
    public Guid PersonId { get; set; }

    [Required(ErrorMessage = "Person name is required")]
    public string? PersonName { get; set; }

    [Required(ErrorMessage = "Email name is required")]
    [EmailAddress(ErrorMessage = "Enter a valid email")]
    public string? Email { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public GenderOptions? Gender { get; set; }
    public Guid? CountryId { get; set; }

    public string? Address { get; set; }

    public bool ReceiveNewsLetters { get; set; }

    public Person ToPerson()
    {
        return new Person()
        {   PersonId=PersonId,
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