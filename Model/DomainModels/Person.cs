using System.ComponentModel.DataAnnotations.Schema;

namespace Model.DomainModels
{
    public class Person
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [NotMapped]
        public string FullName { get {return $"{FirstName} {LastName}"; } }

    }

}
