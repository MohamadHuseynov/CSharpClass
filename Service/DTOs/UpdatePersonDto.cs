namespace Service.DTOs
{
        public class UpdatePersonDto
        {
            // ID is typically passed as a separate parameter to the update service method
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }
    
}
