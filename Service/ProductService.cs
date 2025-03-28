using Model;
using Model.DomainModels;

namespace Service
{
    public class ProductService
    {
        private readonly FinalProjectDbContext _context;

        // سازنده برای Dependency Injection
        public ProductService(FinalProjectDbContext context)
        {
            _context = context;
        }

        // افزودن محصول
        public bool AddProduct(Product product)
        {
            try
            {
                _context.Product.Add(product);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddProduct: {ex.Message}");
                return false;
            }
        }

        // دریافت همه محصولات
        public List<Product> GetAllProducts()
        {
            return _context.Product.ToList();
        }

        // دریافت یک محصول بر اساس ID
        public Product GetProductById(int id)
        {
            return _context.Product.FirstOrDefault(p => p.Id == id);
        }

        // ویرایش محصول
        public bool UpdateProduct(Product product)
        {
            try
            {
                var existingProduct = _context.Product.Find(product.Id);
                if (existingProduct != null)
                {
                    existingProduct.Title = product.Title;
                    existingProduct.UnitPrice = product.UnitPrice;
                    existingProduct.Quantity = product.Quantity;

                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateProduct: {ex.Message}");
                return false;
            }
        }

        // حذف محصول بر اساس ID
        public bool DeleteProduct(int id)
        {
            try
            {
                var product = _context.Product.Find(id);
                if (product != null)
                {
                    _context.Product.Remove(product);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteProduct: {ex.Message}");
                return false;
            }
        }
    }
}

