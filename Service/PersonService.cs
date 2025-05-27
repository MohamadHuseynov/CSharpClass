using Model.DomainModels;
using Model.ServiceModels;
using Service.DTOs;


namespace Service
{
    public class PersonService
    {
        private readonly PersonServiceModel _personServiceModel;

        public PersonService()
        {
            _personServiceModel = new PersonServiceModel();
        }

        // --- Helper Mapping Methods ---
        private GetPersonDto MapEntityToGetPersonDto(Person person) 
        {
            if (person == null) return null;
            return new GetPersonDto
            {
                Id = person.Id,
                FirstName = person.FirstName,
                LastName = person.LastName
                // FullName is a get-only property in GetPersonDto and will be calculated by it
            };
        }

        private Person MapPostDtoToEntity(PostPersonDto postDto)
        {
            if (postDto == null) return null;
            return new Person
            {
                FirstName = postDto.FirstName.Trim(),
                LastName = postDto.LastName.Trim()
            };
        }

        private Person MapUpdateDtoToEntity(int id, UpdatePersonDto updateDto)
        {
            if (updateDto == null) return null;
            return new Person
            {
                Id = id,
                FirstName = updateDto.FirstName.Trim(),
                LastName = updateDto.LastName.Trim()
            };
        }

        // --- Service Methods ---

        public ServiceResult<GetPersonDto> AddPerson(PostPersonDto postPersonDto)
        {
            if (postPersonDto == null)
                return ServiceResult<GetPersonDto>.Fail("Input data cannot be null.");
            if (string.IsNullOrWhiteSpace(postPersonDto.FirstName))
                return ServiceResult<GetPersonDto>.Fail("First name is required.");
            if (string.IsNullOrWhiteSpace(postPersonDto.LastName))
                return ServiceResult<GetPersonDto>.Fail("Last name is required.");

            try
            {
                var personEntity = MapPostDtoToEntity(postPersonDto);
                if (personEntity == null)
                    return ServiceResult<GetPersonDto>.Fail("Failed to map DTO to entity.");

                _personServiceModel.Post(personEntity);

                var createdDto = MapEntityToGetPersonDto(personEntity); // <<< METHOD CALL
                return ServiceResult<GetPersonDto>.Success(createdDto, "Person added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PersonService.AddPerson: {ex.Message}");
                return ServiceResult<GetPersonDto>.Fail($"An error occurred while adding the person: {ex.GetBaseException().Message}");
            }
        }

        public ServiceResult<List<GetPersonDto>> GetAllPersons()
        {
            try
            {
                var personEntities = _personServiceModel.SelectAll();
                var personDtos = personEntities.Select(MapEntityToGetPersonDto).ToList(); // <<< METHOD CALL (as a delegate)
                return ServiceResult<List<GetPersonDto>>.Success(personDtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PersonService.GetAllPersons: {ex.Message}");
                return ServiceResult<List<GetPersonDto>>.Fail($"An error occurred while retrieving persons: {ex.GetBaseException().Message}");
            }
        }

        public ServiceResult<GetPersonDto> GetPersonById(int id)
        {
            if (id <= 0)
                return ServiceResult<GetPersonDto>.Fail("Invalid Person ID.");
            try
            {
                var personEntity = _personServiceModel.SelectById(id);
                if (personEntity == null)
                    return ServiceResult<GetPersonDto>.Fail("Person not found.");

                return ServiceResult<GetPersonDto>.Success(MapEntityToGetPersonDto(personEntity)); // <<< METHOD CALL
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PersonService.GetPersonById: {ex.Message}");
                return ServiceResult<GetPersonDto>.Fail($"An error occurred while retrieving person details: {ex.GetBaseException().Message}");
            }
        }

        public ServiceResult UpdatePerson(int id, UpdatePersonDto updatePersonDto)
        {
            if (id <= 0)
                return ServiceResult.Fail("Invalid Person ID for update.");
            if (updatePersonDto == null)
                return ServiceResult.Fail("Update data cannot be null.");
            if (string.IsNullOrWhiteSpace(updatePersonDto.FirstName))
                return ServiceResult.Fail("First name is required for update.");
            if (string.IsNullOrWhiteSpace(updatePersonDto.LastName))
                return ServiceResult.Fail("Last name is required for update.");

            try
            {
                var personToUpdate = MapUpdateDtoToEntity(id, updatePersonDto);
                if (personToUpdate == null)
                    return ServiceResult.Fail("Failed to map DTO to entity for update.");

                bool success = _personServiceModel.Update(personToUpdate);

                if (success)
                    return ServiceResult.Success("Person updated successfully.");
                else
                    return ServiceResult.Fail("Person not found or update failed at data access level.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PersonService.UpdatePerson: {ex.Message}");
                return ServiceResult.Fail($"An error occurred while updating the person: {ex.GetBaseException().Message}");
            }
        }

        public ServiceResult DeletePerson(int id)
        {
            if (id <= 0)
                return ServiceResult.Fail("Invalid Person ID for deletion.");
            try
            {
                bool success = _personServiceModel.Delete(id);
                if (success)
                    return ServiceResult.Success("Person deleted successfully.");
                else
                    return ServiceResult.Fail("Person not found or delete failed at data access level.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PersonService.DeletePerson: {ex.Message}");
                return ServiceResult.Fail($"An error occurred while deleting the person: {ex.GetBaseException().Message}");
            }
        }
    }
}