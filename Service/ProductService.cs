using Model.DomainModels;    // For Product domain model
using Model.ServiceModels;   // For ProductServiceModel
using Service.DTOs;          // For Product DTOs and ServiceResult


namespace Service
{
    public class ProductService
    {
        private readonly ProductServiceModel _productServiceModel;

        public ProductService()
        {
            _productServiceModel = new ProductServiceModel();
        }

        // --- Helper Mapping Methods ---
        private GetProductDto MapEntityToGetProductDto(Product product)
        {
            if (product == null) return null;
            return new GetProductDto
            {
                Id = product.Id,
                Title = product.Title,
                UnitPrice = product.UnitPrice,
                Quantity = product.Quantity
            };
        }

        // Changed from MapCreateDtoToEntity to MapPostDtoToEntity
        private Product MapPostDtoToEntity(PostProductDto postDto)
        {
            if (postDto == null) return null;
            return new Product
            {
                Title = postDto.Title,
                UnitPrice = postDto.UnitPrice,
                Quantity = postDto.Quantity
            };
        }

        private Product MapUpdateDtoToEntity(int id, UpdateProductDto updateDto)
        {
            if (updateDto == null) return null;
            return new Product
            {
                Id = id, // Set the ID from the parameter
                Title = updateDto.Title,
                UnitPrice = updateDto.UnitPrice,
                Quantity = updateDto.Quantity
            };
        }

        // --- Service Methods ---

        public ServiceResult<GetProductDto> AddProduct(PostProductDto postDto)
        {
            if (postDto == null)
                return ServiceResult<GetProductDto>.Fail("Product data cannot be null.");
            if (string.IsNullOrWhiteSpace(postDto.Title))
                return ServiceResult<GetProductDto>.Fail("Product title is required.");
            if (postDto.UnitPrice < 0)
                return ServiceResult<GetProductDto>.Fail("Unit price cannot be negative.");
            if (postDto.Quantity < 0)
                return ServiceResult<GetProductDto>.Fail("Quantity cannot be negative.");

            try
            {
                var productEntity = MapPostDtoToEntity(postDto); // Uses the renamed mapping method

                _productServiceModel.Post(productEntity);

                var resultDto = MapEntityToGetProductDto(productEntity);
                return ServiceResult<GetProductDto>.Success(resultDto, "Product added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ProductService.AddProduct: {ex.Message}");
                return ServiceResult<GetProductDto>.Fail($"An error occurred while adding the product: {ex.GetBaseException().Message}");
            }
        }

        public ServiceResult<List<GetProductDto>> GetAllProducts()
        {
            try
            {
                var productEntities = _productServiceModel.SelectAll();
                var productDtos = productEntities.Select(MapEntityToGetProductDto).ToList();
                return ServiceResult<List<GetProductDto>>.Success(productDtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ProductService.GetAllProducts: {ex.Message}");
                return ServiceResult<List<GetProductDto>>.Fail($"An error occurred while retrieving products: {ex.GetBaseException().Message}");
            }
        }

        public ServiceResult<GetProductDto> GetProductById(int id)
        {
            if (id <= 0)
                return ServiceResult<GetProductDto>.Fail("Invalid Product ID.");
            try
            {
                var productEntity = _productServiceModel.SelectById(id);
                if (productEntity == null)
                    return ServiceResult<GetProductDto>.Fail("Product not found.");

                return ServiceResult<GetProductDto>.Success(MapEntityToGetProductDto(productEntity));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ProductService.GetProductById: {ex.Message}");
                return ServiceResult<GetProductDto>.Fail($"An error occurred while retrieving product details: {ex.GetBaseException().Message}");
            }
        }

        public ServiceResult UpdateProduct(int id, UpdateProductDto updateDto)
        {
            if (id <= 0)
                return ServiceResult.Fail("Invalid Product ID for update.");
            if (updateDto == null)
                return ServiceResult.Fail("Update data cannot be null.");
            if (string.IsNullOrWhiteSpace(updateDto.Title))
                return ServiceResult.Fail("Product title is required for update.");
            if (updateDto.UnitPrice < 0)
                return ServiceResult.Fail("Unit price cannot be negative for update.");
            if (updateDto.Quantity < 0)
                return ServiceResult.Fail("Quantity cannot be negative for update.");

            try
            {
                var productToUpdate = MapUpdateDtoToEntity(id, updateDto);

                bool success = _productServiceModel.Update(productToUpdate);

                if (success)
                    return ServiceResult.Success("Product updated successfully.");
                else
                    return ServiceResult.Fail("Product not found or update failed at data access level.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ProductService.UpdateProduct: {ex.Message}");
                return ServiceResult.Fail($"An error occurred while updating the product: {ex.GetBaseException().Message}");
            }
        }

        public ServiceResult DeleteProduct(int id)
        {
            if (id <= 0)
                return ServiceResult.Fail("Invalid Product ID for deletion.");
            try
            {
                bool success = _productServiceModel.Delete(id);
                if (success)
                    return ServiceResult.Success("Product deleted successfully.");
                else
                    return ServiceResult.Fail("Product not found or delete failed at data access level.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ProductService.DeleteProduct: {ex.Message}");
                return ServiceResult.Fail($"An error occurred while deleting the product: {ex.GetBaseException().Message}");
            }
        }
    }
}