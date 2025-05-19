using Model;
using Model.DomainModels; 
using Service.DTOs;      
using Microsoft.EntityFrameworkCore; 


namespace Service
{
    public class ProductService : IProductService // Implement the interface
    {
        private readonly FinalProjectDbContext _context;

        public ProductService(FinalProjectDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ServiceResult> AddProductAsync(CreateProductDto productDto)
        {
            // --- Basic DTO Validation (more complex rules can be added) ---
            if (productDto == null)
            {
                return ServiceResult.Fail("Product data for adding is missing.");
            }
            
            if (string.IsNullOrWhiteSpace(productDto.Title))
            {
                return ServiceResult.Fail("Product title is required.");
            }
            if (productDto.UnitPrice < 0)
            {
                return ServiceResult.Fail("Unit price cannot be negative.");
            }
            if (productDto.Quantity < 0)
            {
                return ServiceResult.Fail("Quantity cannot be negative.");
            }

            try
            {
                // --- Mapping from DTO to Entity ---
                var productEntity = new Product
                {
                    Title = productDto.Title.Trim(),
                    UnitPrice = productDto.UnitPrice,
                    Quantity = productDto.Quantity
                };

                _context.Product.Add(productEntity);
                await _context.SaveChangesAsync();

                // Optionally return the ID of the created product
                // return ServiceResult<int>.Success(productEntity.Id, "Product added successfully.");
                return ServiceResult.Success("Product added successfully.");
            }
            catch (DbUpdateException ex)
            {
                // TODO: Log ex (the full exception) with a proper logging framework
                Console.WriteLine($"DbUpdateException in AddProductAsync (Service): {ex.InnerException?.Message ?? ex.Message}");
                return ServiceResult.Fail($"Error saving product to database: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex) // General fallback
            {
                Console.WriteLine($"Error in AddProductAsync (Service): {ex.Message}");
                return ServiceResult.Fail($"An unexpected error occurred while adding the product: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<ProductDto>>> GetAllProductsAsync()
        {
            try
            {
                var productEntities = await _context.Product
                                                    .AsNoTracking() // Good for read-only scenarios
                                                    .ToListAsync();

                // --- Mapping from List<Entity> to List<DTO> ---
                var productDtos = productEntities.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    UnitPrice = p.UnitPrice,
                    Quantity = p.Quantity
                }).ToList();

                return ServiceResult<List<ProductDto>>.Success(productDtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllProductsAsync (Service): {ex.Message}");
                return ServiceResult<List<ProductDto>>.Fail($"Error retrieving product list: {ex.Message}");
            }
        }

        public async Task<ServiceResult<ProductDto>> GetProductByIdAsync(int id)
        {
            if (id <= 0)
            {
                return ServiceResult<ProductDto>.Fail("Invalid product ID.");
            }

            try
            {
                var productEntity = await _context.Product.FindAsync(id);
                // Or: await _context.Product.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id); for read-only

                if (productEntity == null)
                {
                    return ServiceResult<ProductDto>.Fail("Product not found.");
                }

                // --- Mapping from Entity to DTO ---
                var productDto = new ProductDto
                {
                    Id = productEntity.Id,
                    Title = productEntity.Title,
                    UnitPrice = productEntity.UnitPrice,
                    Quantity = productEntity.Quantity
                };
                return ServiceResult<ProductDto>.Success(productDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetProductByIdAsync (Service): {ex.Message}");
                return ServiceResult<ProductDto>.Fail($"Error retrieving product details: {ex.Message}");
            }
        }

        public async Task<ServiceResult> UpdateProductAsync(int id, UpdateProductDto productDto)
        {
            if (id <= 0)
            {
                return ServiceResult.Fail("Invalid product ID for update.");
            }
            if (productDto == null)
            {
                return ServiceResult.Fail("Product data for update is missing.");
            }
            if (string.IsNullOrWhiteSpace(productDto.Title))
            {
                return ServiceResult.Fail("Product title is required for update.");
            }
            if (productDto.UnitPrice < 0)
            {
                return ServiceResult.Fail("Unit price cannot be negative for update.");
            }
            if (productDto.Quantity < 0)
            {
                return ServiceResult.Fail("Quantity cannot be negative for update.");
            }

            try
            {
                var existingProduct = await _context.Product.FindAsync(id);
                if (existingProduct == null)
                {
                    return ServiceResult.Fail("Product to update not found.");
                }

                // --- Mapping from DTO to existing Entity's properties ---
                existingProduct.Title = productDto.Title.Trim();
                existingProduct.UnitPrice = productDto.UnitPrice;
                existingProduct.Quantity = productDto.Quantity;

                // EF Core tracks changes on the existingProduct entity
                await _context.SaveChangesAsync();
                return ServiceResult.Success("Product updated successfully.");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine($"Concurrency Error in UpdateProductAsync (Service): {ex.Message}");
                return ServiceResult.Fail("The product data was modified by another user. Please refresh and try again.");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException in UpdateProductAsync (Service): {ex.InnerException?.Message ?? ex.Message}");
                return ServiceResult.Fail($"Error saving product update: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateProductAsync (Service): {ex.Message}");
                return ServiceResult.Fail($"An unexpected error occurred while updating the product: {ex.Message}");
            }
        }

        public async Task<ServiceResult> DeleteProductAsync(int id)
        {
            if (id <= 0)
            {
                return ServiceResult.Fail("Invalid product ID for deletion.");
            }

            try
            {
                var product = await _context.Product.FindAsync(id);
                if (product == null)
                {
                    return ServiceResult.Fail("Product to delete not found.");
                }

                _context.Product.Remove(product);
                await _context.SaveChangesAsync();
                return ServiceResult.Success("Product deleted successfully.");
            }
            catch (DbUpdateException ex) // Catches issues like foreign key constraints
            {
                Console.WriteLine($"Database Error in DeleteProductAsync (Service): {ex.InnerException?.Message ?? ex.Message}");
                return ServiceResult.Fail("Error deleting product. It might be referenced by other data in the system.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteProductAsync (Service): {ex.Message}");
                return ServiceResult.Fail($"An unexpected error occurred while deleting the product: {ex.Message}");
            }
        }
    }
}