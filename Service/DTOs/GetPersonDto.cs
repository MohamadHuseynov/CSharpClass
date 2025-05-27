using System.ComponentModel.DataAnnotations.Schema;

namespace Service.DTOs
{
    public class GetPersonDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [NotMapped]

        public string FullName { get { return $"{FirstName} {LastName}"; } }
    }
}



