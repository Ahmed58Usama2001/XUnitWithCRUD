using ServiceContracts.DTO;
using ServiceContracts.DTO.Enums;

namespace ServiceContracts;

public interface IPersonService
{
    Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);

    Task<List<PersonResponse>> GetAllPersons();

    Task<PersonResponse?> GetPersonByPersonId(Guid? personId);

    Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);

    Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> persons , string sortBy , SortOrderOptions sortOrder);

    Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);

    Task<bool> DeletePerson (Guid? personId);
}
