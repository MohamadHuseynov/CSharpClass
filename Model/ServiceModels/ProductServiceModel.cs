// Namespace for Service Models. In this specific architecture, these models contain data access logic.
using Model.DomainModels; // Required to use the Product domain entity.
using Microsoft.EntityFrameworkCore; // Required for Entity Framework Core features like AsNoTracking() and Find().
// System, System.Collections.Generic, System.Linq are implicitly available or used for List<T> and FirstOrDefault.
using System; // For Exception handling and Console.WriteLine.
using System.Collections.Generic; // For List<T>.
using System.Linq; // For .ToList() and .FirstOrDefault().

namespace Model.ServiceModels
{
    /// <summary>
    /// Provides data access services for Product entities.
    /// This class directly interacts with the database using an instance of FinalProjectDbContext.
    /// As per the established pattern, a new DbContext is created and disposed for each public method call,
    /// ensuring that each operation is a distinct unit of work.
    /// </summary>
    public class ProductServiceModel
    {
        /// <summary>
        /// Adds a new product to the database.
        /// The Id property of the passed 'product' object will be populated by EF Core
        /// upon successful insertion if the database is configured to generate IDs (e.g., identity column).
        /// </summary>
        /// <param name="product">The Product entity to add to the database.</param>
        public void Post(Product product)
        {
            // A new DbContext instance is created for the scope of this method.
            // The 'using' statement ensures that the DbContext is properly disposed of
            // when the method execution completes, even if errors occur.
            using (var context = new FinalProjectDbContext())
            {
                try
                {
                    // Add the product entity to the DbContext's Product DbSet.
                    // This marks the entity to be inserted into the database.
                    context.Product.Add(product);
                    // Persist all tracked changes (in this case, the new product) to the database.
                    context.SaveChanges();
                    // After SaveChanges, if 'product.Id' is an identity column, 
                    // EF Core will automatically populate it with the database-generated ID.
                }
                catch (Exception ex)
                {
                    // If an error occurs during database interaction (e.g., connection issue, constraint violation),
                    // log the error message to the console for debugging purposes.
                    Console.WriteLine($"Error in ProductServiceModel.Post: {ex.Message}");
                    // Re-throw the original exception. This allows the calling layer (e.g., ProductService)
                    // to be aware of the failure and handle it appropriately (e.g., return a failed ServiceResult).
                    throw;
                }
            }
        }

        /// <summary>
        /// Retrieves all product records from the database.
        /// </summary>
        /// <returns>A List of Product entities.</returns>
        public List<Product> SelectAll()
        {
            using (var context = new FinalProjectDbContext())
            {
                try
                {
                    // Retrieve all entities from the Product DbSet.
                    // AsNoTracking() is an optimization for read-only queries; it tells EF Core
                    // not to track changes for the retrieved entities, which can improve performance.
                    return context.Product.AsNoTracking().ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in ProductServiceModel.SelectAll: {ex.Message}");
                    throw;
                }
            }
        }

        /// <summary>
        /// Retrieves a specific product by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the product to retrieve.</param>
        /// <returns>The Product entity if found; otherwise, null.</returns>
        public Product SelectById(int id)
        {
            using (var context = new FinalProjectDbContext())
            {
                try
                {
                    // Retrieve the first product entity that matches the given ID.
                    // AsNoTracking() is used for this read-only query.
                    // FirstOrDefault returns null if no matching entity is found, rather than throwing an exception.
                    return context.Product.AsNoTracking().FirstOrDefault(p => p.Id == id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in ProductServiceModel.SelectById: {ex.Message}");
                    throw;
                }
            }
        }

        /// <summary>
        /// Updates an existing product in the database.
        /// </summary>
        /// <param name="productToUpdate">The Product entity containing the ID of the product to update
        /// and its new values for Title, UnitPrice, and Quantity.</param>
        /// <returns>True if the product was found and updated successfully; false if the product was not found.</returns>
        public bool Update(Product productToUpdate)
        {
            using (var context = new FinalProjectDbContext())
            {
                try
                {
                    // Attempt to find the existing product in the database using its ID.
                    // The Find() method first checks if the entity is already tracked by the context
                    // and if not, queries the database.
                    var existingProduct = context.Product.Find(productToUpdate.Id);

                    if (existingProduct == null)
                    {
                        // If no product with the given ID exists, the update cannot proceed.
                        return false;
                    }

                    // Apply the updated values from productToUpdate to the properties of the existingProduct entity.
                    // Because existingProduct is tracked by the DbContext, EF Core will detect these changes.
                    existingProduct.Title = productToUpdate.Title;
                    existingProduct.UnitPrice = productToUpdate.UnitPrice;
                    existingProduct.Quantity = productToUpdate.Quantity;

                    // Persist the changes to the database.
                    context.SaveChanges();
                    return true; // Update was successful.
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in ProductServiceModel.Update: {ex.Message}");
                    throw;
                }
            }
        }

        /// <summary>
        /// Deletes a product from the database based on its ID.
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        /// <returns>True if the product was found and deleted successfully; false if the product was not found.</returns>
        public bool Delete(int id)
        {
            using (var context = new FinalProjectDbContext())
            {
                try
                {
                    // Attempt to find the product to delete by its ID.
                    var productToDelete = context.Product.Find(id);

                    if (productToDelete == null)
                    {
                        // If no product with the given ID exists, the deletion cannot proceed.
                        return false;
                    }

                    // Mark the found entity for removal from the database.
                    context.Product.Remove(productToDelete);
                    // Persist the deletion to the database.
                    context.SaveChanges();
                    return true; // Deletion was successful.
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in ProductServiceModel.Delete: {ex.Message}");
                    throw;
                }
            }
        }
    }
}