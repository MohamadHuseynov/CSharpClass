using Model.DomainModels;
using Microsoft.EntityFrameworkCore; // For AsNoTracking, Find etc.


namespace Model.ServiceModels
{
    public class ProductServiceModel
    {
        public void Post(Product product)
        {
            using (var context = new FinalProjectDbContext())
            {
                try
                {
                    context.Product.Add(product);
                    context.SaveChanges();
                    // product.Id will be populated by EF Core
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in ProductServiceModel.Post: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Product> SelectAll()
        {
            using (var context = new FinalProjectDbContext())
            {
                try
                {
                    return context.Product.AsNoTracking().ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in ProductServiceModel.SelectAll: {ex.Message}");
                    throw;
                }
            }
        }

        public Product SelectById(int id)
        {
            using (var context = new FinalProjectDbContext())
            {
                try
                {
                    return context.Product.AsNoTracking().FirstOrDefault(p => p.Id == id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in ProductServiceModel.SelectById: {ex.Message}");
                    throw;
                }
            }
        }

        public bool Update(Product productToUpdate)
        {
            using (var context = new FinalProjectDbContext())
            {
                try
                {
                    var existingProduct = context.Product.Find(productToUpdate.Id);
                    if (existingProduct == null)
                    {
                        return false; // Product not found
                    }

                    existingProduct.Title = productToUpdate.Title;
                    existingProduct.UnitPrice = productToUpdate.UnitPrice;
                    existingProduct.Quantity = productToUpdate.Quantity;

                    context.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in ProductServiceModel.Update: {ex.Message}");
                    throw;
                }
            }
        }

        public bool Delete(int id)
        {
            using (var context = new FinalProjectDbContext())
            {
                try
                {
                    var productToDelete = context.Product.Find(id);
                    if (productToDelete == null)
                    {
                        return false; // Product not found
                    }
                    context.Product.Remove(productToDelete);
                    context.SaveChanges();
                    return true;
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