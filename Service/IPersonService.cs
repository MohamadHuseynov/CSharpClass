using Service.DTOs; 
using System.Collections.Generic;
using System.Threading.Tasks; // For Task

namespace Service
{
    public interface IPersonService
    {
        Task<ServiceResult> AddPersonAsync(CreatePersonDto personDto);
        Task<ServiceResult<List<PersonDto>>> GetAllPersonsAsync();
        Task<ServiceResult<PersonDto>> GetPersonByIdAsync(int id);
        Task<ServiceResult> UpdatePersonAsync(int id, UpdatePersonDto personDto);
        Task<ServiceResult> DeletePersonAsync(int id);
        // Add any other public methods from PersonService that the View needs
    }
}