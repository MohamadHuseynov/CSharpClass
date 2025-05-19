using Service.DTOs; 


namespace Service
{
    public interface IPersonService
    {
        Task<ServiceResult> AddPersonAsync(CreatePersonDto personDto);
        Task<ServiceResult<List<PersonDto>>> GetAllPersonsAsync();
        Task<ServiceResult<PersonDto>> GetPersonByIdAsync(int id);
        Task<ServiceResult> UpdatePersonAsync(int id, UpdatePersonDto personDto);
        Task<ServiceResult> DeletePersonAsync(int id);
        
    }
}