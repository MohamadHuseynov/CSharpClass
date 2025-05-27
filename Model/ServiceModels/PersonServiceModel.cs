using Model.DomainModels;
using Microsoft.EntityFrameworkCore; // For AsNoTracking, Find etc.


namespace Model.ServiceModels
{
    public class PersonServiceModel
    {
        public void Post(Person person) // Your existing Post logic
        {
            using (var context = new Model.FinalProjectDbContext())
            {
                try
                {
                    context.Person.Add(person);
                    context.SaveChanges();
                    // person.Id will be populated by EF Core after SaveChanges
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in PersonServiceModel.Post: {ex.Message}");
                    throw; // Re-throw to be handled by the service layer
                }
            }
        }

        public List<Person> SelectAll() // Your existing SelectAll logic
        {
            using (var context = new Model.FinalProjectDbContext())
            {
                try
                {
                    return context.Person.AsNoTracking().ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in PersonServiceModel.SelectAll: {ex.Message}");
                    throw;
                }
            }
        }

        public Person SelectById(int id)
        {
            using (var context = new Model.FinalProjectDbContext())
            {
                try
                {
                    return context.Person.AsNoTracking().FirstOrDefault(p => p.Id == id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in PersonServiceModel.SelectById: {ex.Message}");
                    throw;
                }
            }
        }

        public bool Update(Person personToUpdate)
        {
            using (var context = new Model.FinalProjectDbContext())
            {
                try
                {
                    var existingPerson = context.Person.Find(personToUpdate.Id);
                    if (existingPerson == null)
                    {
                        return false; // Person not found
                    }

                    existingPerson.FirstName = personToUpdate.FirstName;
                    existingPerson.LastName = personToUpdate.LastName;

                    context.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in PersonServiceModel.Update: {ex.Message}");
                    throw; // Re-throw to be handled by the service layer
                }
            }
        }

        public bool Delete(int id)
        {
            using (var context = new Model.FinalProjectDbContext())
            {
                try
                {
                    var personToDelete = context.Person.Find(id);
                    if (personToDelete == null)
                    {
                        return false; // Person not found
                    }
                    context.Person.Remove(personToDelete);
                    context.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in PersonServiceModel.Delete: {ex.Message}");
                    throw; // Re-throw to be handled by the service layer
                }
            }
        }
    }
}