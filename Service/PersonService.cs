// Namespace for service layer classes, which act as intermediaries.
using Model.DomainModels;    // To use the Person domain entity.
using Model.ServiceModels;   // To use PersonServiceModel for data access.
using Service.DTOs;          // To use Person DTOs (Post, Get, Update) and ServiceResult.
using System;                // For Exception handling (though try-catch is removed) and Console.WriteLine (if used for non-exception logging).
using System.Collections.Generic; // For List<T>.
using System.Linq;                // For LINQ methods like .Select().

namespace Service
{
    /// <summary>
    /// Service layer for managing person-related operations.
    /// This service orchestrates data flow between the View layer (e.g., frmPerson)
    /// and the data access component (PersonServiceModel). It handles DTO mapping,
    /// input validation, and wraps results in ServiceResult objects.
    /// Note: In this version, try-catch blocks have been removed; exceptions from PersonServiceModel
    /// will propagate to the caller (typically the View layer).
    /// </summary>
    public class PersonService
    {
        // Instance of PersonServiceModel to handle direct database interactions for Person entities.
        // Marked readonly as it's initialized in the constructor and not changed afterwards.
        private readonly PersonServiceModel _personServiceModel;

        /// <summary>
        /// Initializes a new instance of the PersonService class.
        /// In this design, it creates its own instance of PersonServiceModel.
        /// </summary>
        public PersonService()
        {
            _personServiceModel = new PersonServiceModel();
        }

        // --- Private Helper Mapping Methods ---

        /// <summary>
        /// Maps a Person domain entity to a GetPersonDto.
        /// </summary>
        /// <param name="person">The Person entity to map from.</param>
        /// <returns>A GetPersonDto representation of the person, or null if the input is null.</returns>
        private GetPersonDto MapEntityToGetPersonDto(Person person)
        {
            if (person == null) return null; // Guard clause for null input.
            return new GetPersonDto
            {
                Id = person.Id,
                FirstName = person.FirstName,
                LastName = person.LastName
                // The FullName property in GetPersonDto is a calculated get-only property,
                // so it does not need to be explicitly mapped here.
            };
        }

        /// <summary>
        /// Maps a PostPersonDto to a new Person domain entity.
        /// </summary>
        /// <param name="postDto">The PostPersonDto containing data for the new person.</param>
        /// <returns>A Person entity, or null if the input DTO is null.</returns>
        private Person MapPostDtoToEntity(PostPersonDto postDto)
        {
            if (postDto == null) return null;
            return new Person
            {
                // Trim string inputs to remove accidental leading/trailing whitespace.
                FirstName = postDto.FirstName.Trim(),
                LastName = postDto.LastName.Trim()
            };
        }

        /// <summary>
        /// Maps an UpdatePersonDto and an ID to a Person domain entity, typically for an update operation.
        /// </summary>
        /// <param name="id">The ID of the person being updated.</param>
        /// <param name="updateDto">The UpdatePersonDto containing the new values.</param>
        /// <returns>A Person entity with the ID and updated values, or null if the input DTO is null.</returns>
        private Person MapUpdateDtoToEntity(int id, UpdatePersonDto updateDto)
        {
            if (updateDto == null) return null;
            return new Person
            {
                Id = id, // The ID is crucial for identifying the entity to update.
                FirstName = updateDto.FirstName.Trim(),
                LastName = updateDto.LastName.Trim()
            };
        }

        // --- Public Service Methods ---

        /// <summary>
        /// Adds a new person to the system.
        /// Exceptions from the data access layer will propagate.
        /// </summary>
        /// <param name="postPersonDto">The DTO containing the details of the person to add.</param>
        /// <returns>A ServiceResult containing the GetPersonDto of the newly added person on success,
        /// or an error message on validation failure.</returns>
        public ServiceResult<GetPersonDto> AddPerson(PostPersonDto postPersonDto)
        {
            // --- Input Validation ---
            if (postPersonDto == null)
                return ServiceResult<GetPersonDto>.Fail("Input data cannot be null.");
            if (string.IsNullOrWhiteSpace(postPersonDto.FirstName))
                return ServiceResult<GetPersonDto>.Fail("First name is required.");
            if (string.IsNullOrWhiteSpace(postPersonDto.LastName))
                return ServiceResult<GetPersonDto>.Fail("Last name is required.");

            // Map the DTO to a domain entity.
            var personEntity = MapPostDtoToEntity(postPersonDto);
            if (personEntity == null)
                return ServiceResult<GetPersonDto>.Fail("Failed to map DTO to entity.");

            // Call the data access layer to persist the person.
            _personServiceModel.Post(personEntity);

            // Map the persisted entity (now with an ID) back to a DTO for the result.
            var createdDto = MapEntityToGetPersonDto(personEntity);
            return ServiceResult<GetPersonDto>.Success(createdDto, "Person added successfully.");
        }

        /// <summary>
        /// Retrieves all persons from the system.
        /// Exceptions from the data access layer will propagate.
        /// </summary>
        /// <returns>A ServiceResult containing a list of GetPersonDto on success,
        /// or an error message on failure (if an exception is caught by a higher layer).</returns>
        public ServiceResult<List<GetPersonDto>> GetAllPersons()
        {
            // Fetch all person entities from the data access layer.
            var personEntities = _personServiceModel.SelectAll();
            // Map the list of domain entities to a list of DTOs.
            var personDtos = personEntities.Select(MapEntityToGetPersonDto).ToList();
            return ServiceResult<List<GetPersonDto>>.Success(personDtos);
        }

        /// <summary>
        /// Retrieves a specific person by their ID.
        /// Exceptions from the data access layer will propagate.
        /// </summary>
        /// <param name="id">The ID of the person to retrieve.</param>
        /// <returns>A ServiceResult containing the GetPersonDto for the found person on success,
        /// or an error message if not found or on other failures (if an exception is caught by a higher layer).</returns>
        public ServiceResult<GetPersonDto> GetPersonById(int id)
        {
            if (id <= 0) // Basic ID validation.
                return ServiceResult<GetPersonDto>.Fail("Invalid Person ID.");

            var personEntity = _personServiceModel.SelectById(id);
            if (personEntity == null)
                return ServiceResult<GetPersonDto>.Fail("Person not found.");

            // Map the found entity to a DTO and return a successful result.
            return ServiceResult<GetPersonDto>.Success(MapEntityToGetPersonDto(personEntity));
        }

        /// <summary>
        /// Updates an existing person.
        /// Exceptions from the data access layer will propagate.
        /// </summary>
        /// <param name="id">The ID of the person to be updated.</param>
        /// <param name="updatePersonDto">The DTO containing the new values for the person.</param>
        /// <returns>A ServiceResult indicating the success or failure of the update operation.</returns>
        public ServiceResult UpdatePerson(int id, UpdatePersonDto updatePersonDto)
        {
            // --- Input Validation ---
            if (id <= 0)
                return ServiceResult.Fail("Invalid Person ID for update.");
            if (updatePersonDto == null)
                return ServiceResult.Fail("Update data cannot be null.");
            if (string.IsNullOrWhiteSpace(updatePersonDto.FirstName))
                return ServiceResult.Fail("First name is required for update.");
            if (string.IsNullOrWhiteSpace(updatePersonDto.LastName))
                return ServiceResult.Fail("Last name is required for update.");

            // Map the DTO and ID to a Person entity for the update operation.
            var personToUpdate = MapUpdateDtoToEntity(id, updatePersonDto);
            if (personToUpdate == null)
                return ServiceResult.Fail("Failed to map DTO to entity for update.");

            // Call the data access layer to perform the update.
            bool success = _personServiceModel.Update(personToUpdate);

            if (success)
                return ServiceResult.Success("Person updated successfully.");
            else
                return ServiceResult.Fail("Person not found or update failed at data access level.");
        }

        /// <summary>
        /// Deletes a person by their ID.
        /// Exceptions from the data access layer will propagate.
        /// </summary>
        /// <param name="id">The ID of the person to be deleted.</param>
        /// <returns>A ServiceResult indicating the success or failure of the delete operation.</returns>
        public ServiceResult DeletePerson(int id)
        {
            if (id <= 0) // Basic ID validation.
                return ServiceResult.Fail("Invalid Person ID for deletion.");

            bool success = _personServiceModel.Delete(id);
            if (success)
                return ServiceResult.Success("Person deleted successfully.");
            else
                return ServiceResult.Fail("Person not found or delete failed at data access level.");
        }
    }
}