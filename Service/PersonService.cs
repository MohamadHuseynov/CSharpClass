using Model;
using Model.DomainModels;

namespace Service
{
    public class PersonService
    {
        private readonly FinalProjectDbContext _context;

        // سازنده برای Dependency Injection
        public PersonService(FinalProjectDbContext context)
        {
            _context = context;
        }

        // افزودن کاربر
        public bool AddPerson(Person person)
        {
            try
            {
                _context.Person.Add(person);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddPerson: {ex.Message}");
                return false;
            }
        }

        // دریافت همه کاربرها
        public List<Person> GetAllPersons()
        {
            return _context.Person.ToList();
        }

        // دریافت یک کاربر بر اساس ID
        public Person GetPersonById(int id)
        {
            return _context.Person.FirstOrDefault(p => p.Id == id);
        }

        // ویرایش کاربر
        public bool UpdatePerson(Person person)
        {
            try
            {
                var existingPerson = _context.Person.Find(person);
                if (existingPerson != null)
                {
                    existingPerson.FirstName = person.FirstName;
                    existingPerson.LastName = person.LastName;
                   

                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdatePerson: {ex.Message}");
                return false;
            }
        }

        // حذف کاربر بر اساس ID
        public bool DeletePerson(int id)
        {
            try
            {
                var person = _context.Person.Find(id);
                if (person != null)
                {
                    _context.Person.Remove(person);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeletePerson: {ex.Message}");
                return false;
            }
        }
    }
}

