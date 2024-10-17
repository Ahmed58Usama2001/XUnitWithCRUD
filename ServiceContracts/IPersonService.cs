using ServiceContracts.DTO;
using ServiceContracts.DTO.Enums;

namespace ServiceContracts;

public interface IPersonService
{
    PersonResponse AddPerson(PersonAddRequest? personAddRequest);

    List<PersonResponse> GetAllPersons();

    PersonResponse? GetPersonByPersonId(Guid? personId);

    List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString);

    List<PersonResponse> GetSortedPersons(List<PersonResponse> persons , string sortBy , SortOrderOptions sortOrder);

    PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest);

    bool DeletePerson (Guid? personId);
}
