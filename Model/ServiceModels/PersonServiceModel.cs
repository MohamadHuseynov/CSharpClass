// Namespace for Service Models. In this specific architecture, these models contain data access logic.
using Model.DomainModels; // Required to use the Person domain entity.
using Microsoft.EntityFrameworkCore; // Required for Entity Framework Core features like AsNoTracking() and Find().
// System, System.Collections.Generic, System.Linq are implicitly available or not strictly needed for direct features used here beyond List<T> and FirstOrDefault.

namespace Model.ServiceModels
{
    /// <summary>
    /// Provides data access services for Person entities.
    /// This class directly interacts with the database using an instance of FinalProjectDbContext.
    /// As per the established pattern, a new DbContext is created and disposed for each public method call,
    /// ensuring that each operation is a distinct unit of work.
    /// </summary>
    public class PersonServiceModel
    {
        /// <summary>
        /// Adds a new person to the database.
        /// The Id property of the passed 'person' object will be populated by EF Core
        /// upon successful insertion if the database is configured to generate IDs (e.g., identity column).
        /// </summary>
        /// <param name="person">The Person entity to add to the database.</param>
        public void Post(Person person)
        {
            // A new DbContext instance is created for the scope of this method.
            // The 'using' statement ensures that the DbContext is properly disposed of
            // when the method execution completes, even if errors occur.
            using (var context = new Model.FinalProjectDbContext())
            {
                try
                {
                    // Add the person entity to the DbContext's Person DbSet.
                    // This marks the entity to be inserted into the database.
                    context.Person.Add(person);
                    // Persist all tracked changes (in this case, the new person) to the database.
                    context.SaveChanges();
                    // After SaveChanges, if 'person.Id' is an identity column, 
                    // EF Core will automatically populate it with the database-generated ID.
                }
                catch (Exception ex)
                {
                    // If an error occurs during database interaction (e.g., connection issue, constraint violation),
                    // log the error message to the console for debugging purposes.
                    Console.WriteLine($"Error in PersonServiceModel.Post: {ex.Message}");
                    // Re-throw the original exception. This allows the calling layer (e.g., PersonService)
                    // to be aware of the failure and handle it appropriately (e.g., return a failed ServiceResult).
                    throw;
                }
            }
        }

        /// <summary>
        /// Retrieves all person records from the database.
        /// </summary>
        /// <returns>A List of Person entities.</returns>
        public List<Person> SelectAll()
        {
            using (var context = new Model.FinalProjectDbContext())
            {
                try
                {
                    // Retrieve all entities from the Person DbSet.
                    // AsNoTracking() is an optimization for read-only queries; it tells EF Core
                    // not to track changes for the retrieved entities, which can improve performance.
                    return context.Person.AsNoTracking().ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in PersonServiceModel.SelectAll: {ex.Message}");
                    throw;
                }
            }
        }

        /// <summary>
        /// Retrieves a specific person by their unique identifier.
        /// </summary>
        /// <param name="id">The ID of the person to retrieve.</param>
        /// <returns>The Person entity if found; otherwise, null.</returns>
        public Person SelectById(int id)
        {
            using (var context = new Model.FinalProjectDbContext())
            {
                try
                {
                    // Retrieve the first person entity that matches the given ID.
                    // AsNoTracking() is used for this read-only query.
                    // FirstOrDefault returns null if no matching entity is found, rather than throwing an exception.
                    return context.Person.AsNoTracking().FirstOrDefault(p => p.Id == id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in PersonServiceModel.SelectById: {ex.Message}");
                    throw;
                }
            }
        }

        /// <summary>
        /// Updates an existing person in the database.
        /// </summary>
        /// <param name="personToUpdate">The Person entity containing the ID of the person to update
        /// and its new values for FirstName and LastName.</param>
        /// <returns>True if the person was found and updated successfully; false if the person was not found.</returns>
        public bool Update(Person personToUpdate)
        {
            using (var context = new Model.FinalProjectDbContext())
            {
                try
                {
                    // Attempt to find the existing person in the database using their ID.
                    // The Find() method first checks if the entity is already tracked by the context
                    // and if not, queries the database.
                    var existingPerson = context.Person.Find(personToUpdate.Id);

                    if (existingPerson == null)
                    {
                        // If no person with the given ID exists, the update cannot proceed.
                        return false;
                    }

                    // Apply the updated values from personToUpdate to the properties of the existingPerson entity.
                    // Because existingPerson is tracked by the DbContext, EF Core will detect these changes.
                    existingPerson.FirstName = personToUpdate.FirstName;
                    existingPerson.LastName = personToUpdate.LastName;

                    // Persist the changes to the database.
                    context.SaveChanges();
                    return true; // Update was successful.
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in PersonServiceModel.Update: {ex.Message}");
                    throw;
                }
            }
        }

        /// <summary>
        /// Deletes a person from the database based on their ID.
        /// </summary>
        /// <param name="id">The ID of the person to delete.</param>
        /// <returns>True if the person was found and deleted successfully; false if the person was not found.</returns>
        public bool Delete(int id)
        {
            using (var context = new Model.FinalProjectDbContext())
            {
                try
                {
                    // Attempt to find the person to delete by their ID.
                    var personToDelete = context.Person.Find(id);

                    if (personToDelete == null)
                    {
                        // If no person with the given ID exists, the deletion cannot proceed.
                        return false;
                    }

                    // Mark the found entity for removal from the database.
                    context.Person.Remove(personToDelete);
                    // Persist the deletion to the database.
                    context.SaveChanges();
                    return true; // Deletion was successful.
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in PersonServiceModel.Delete: {ex.Message}");
                    throw;
                }
            }
        }
    }
}