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

        // Changed from MapCreateDtoToEntity to MapPostDtoToEntity// Namespace for service layer classes, which act as intermediaries.
        // using Model.DomainModels;    // To use the Product domain entity.
        // using Model.ServiceModels;   // To use ProductServiceModel for data access.
        // using Service.DTOs;          // To use Product DTOs (Post, Get, Update) and ServiceResult.
        // using System;                // For Exception handling and Console.WriteLine.
        // using System.Collections.Generic; // For List<T>.
        // using System.Linq;                // For LINQ methods like .Select().
        // 
        // namespace Service
        // {
        //     /// <summary>
        //     /// Service layer for managing product-related operations.
        //     /// This service orchestrates data flow between the View layer (e.g., frmProduct)
        //     /// and the data access component (ProductServiceModel). It handles DTO mapping,
        //     /// input validation, and wraps results in ServiceResult objects.
        //     /// </summary>
        //     public class ProductService
        //     {
        //         // Instance of ProductServiceModel to handle direct database interactions for Product entities.
        //         // Marked readonly as it's initialized in the constructor and not changed afterwards.
        //         private readonly ProductServiceModel _productServiceModel;
        // 
        //         /// <summary>
        //         /// Initializes a new instance of the ProductService class.
        //         /// In this design, it creates its own instance of ProductServiceModel.
        //         /// </summary>
        //         public ProductService()
        //         {
        //             _productServiceModel = new ProductServiceModel();
        //         }
        // 
        //         // --- Private Helper Mapping Methods ---
        // 
        //         /// <summary>
        //         /// Maps a Product domain entity to a GetProductDto.
        //         /// </summary>
        //         /// <param name="product">The Product entity to map from.</param>
        //         /// <returns>A GetProductDto representation of the product, or null if the input is null.</returns>
        //         private GetProductDto MapEntityToGetProductDto(Product product)
        //         {
        //             if (product == null) return null; // Guard clause for null input.
        //             return new GetProductDto
        //             {
        //                 Id = product.Id,
        //                 Title = product.Title,
        //                 UnitPrice = product.UnitPrice,
        //                 Quantity = product.Quantity
        //             };
        //         }
        // 
        //         /// <summary>
        //         /// Maps a PostProductDto to a new Product domain entity.
        //         /// (Method name reflects user preference for "Post" over "Create" for DTOs).
        //         /// </summary>
        //         /// <param name="postDto">The PostProductDto containing data for the new product.</param>
        //         /// <returns>A Product entity, or null if the input DTO is null.</returns>
        //         private Product MapPostDtoToEntity(PostProductDto postDto)
        //         {
        //             if (postDto == null) return null;
        //             return new Product
        //             {
        //                 // Trim string inputs if necessary, though not explicitly done here for Title.
        //                 // Assuming Title from DTO is already suitable or validation occurs elsewhere.
        //                 Title = postDto.Title, 
        //                 UnitPrice = postDto.UnitPrice,
        //                 Quantity = postDto.Quantity
        //             };
        //         }
        // 
        //         /// <summary>
        //         /// Maps an UpdateProductDto and an ID to a Product domain entity, typically for an update operation.
        //         /// </summary>
        //         /// <param name="id">The ID of the product being updated.</param>
        //         /// <param name="updateDto">The UpdateProductDto containing the new values.</param>
        //         /// <returns>A Product entity with the ID and updated values, or null if the input DTO is null.</returns>
        //         private Product MapUpdateDtoToEntity(int id, UpdateProductDto updateDto)
        //         {
        //             if (updateDto == null) return null;
        //             return new Product
        //             {
        //                 Id = id, // The ID is crucial for identifying the entity to update.
        //                 Title = updateDto.Title,
        //                 UnitPrice = updateDto.UnitPrice,
        //                 Quantity = updateDto.Quantity
        //             };
        //         }
        // 
        //         // --- Public Service Methods ---
        // 
        //         /// <summary>
        //         /// Adds a new product to the system.
        //         /// </summary>
        //         /// <param name="postDto">The DTO containing the details of the product to add.</param>
        //         /// <returns>A ServiceResult containing the GetProductDto of the newly added product on success,
        //         /// or an error message on failure.</returns>
        //         public ServiceResult<GetProductDto> AddProduct(PostProductDto postDto)
        //         {
        //             // --- Input Validation ---
        //             if (postDto == null)
        //                 return ServiceResult<GetProductDto>.Fail("Product data cannot be null.");
        //             if (string.IsNullOrWhiteSpace(postDto.Title))
        //                 return ServiceResult<GetProductDto>.Fail("Product title is required.");
        //             if (postDto.UnitPrice < 0)
        //                 return ServiceResult<GetProductDto>.Fail("Unit price cannot be negative.");
        //             if (postDto.Quantity < 0)
        //                 return ServiceResult<GetProductDto>.Fail("Quantity cannot be negative.");
        // 
        //             try
        //             {
        //                 // Map the DTO to a domain entity.
        //                 var productEntity = MapPostDtoToEntity(postDto); 
        //                 // Note: productEntity will not have an ID yet if it's a new product.
        // 
        //                 // Call the data access layer to persist the product.
        //                 // The Post method in ProductServiceModel handles SaveChanges, and EF Core populates productEntity.Id.
        //                 _productServiceModel.Post(productEntity);
        // 
        //                 // Map the persisted entity (now with an ID) back to a DTO for the result.
        //                 var resultDto = MapEntityToGetProductDto(productEntity);
        //                 return ServiceResult<GetProductDto>.Success(resultDto, "Product added successfully.");
        //             }
        //             catch (Exception ex)
        //             {
        //                 // Log the exception (e.g., to a logging framework or console).
        //                 Console.WriteLine($"Error in ProductService.AddProduct: {ex.Message}");
        //                 // Return a failure result with a user-friendly message (or the base exception message for more details).
        //                 return ServiceResult<GetProductDto>.Fail($"An error occurred while adding the product: {ex.GetBaseException().Message}");
        //             }
        //         }
        // 
        //         /// <summary>
        //         /// Retrieves all products from the system.
        //         /// </summary>
        //         /// <returns>A ServiceResult containing a list of GetProductDto on success,
        //         /// or an error message on failure.</returns>
        //         public ServiceResult<List<GetProductDto>> GetAllProducts()
        //         {
        //             try
        //             {
        //                 // Fetch all product entities from the data access layer.
        //                 var productEntities = _productServiceModel.SelectAll();
        //                 // Map the list of domain entities to a list of DTOs using LINQ .Select and the mapping helper.
        //                 var productDtos = productEntities.Select(MapEntityToGetProductDto).ToList();
        //                 return ServiceResult<List<GetProductDto>>.Success(productDtos);
        //             }
        //             catch (Exception ex)
        //             {
        //                 Console.WriteLine($"Error in ProductService.GetAllProducts: {ex.Message}");
        //                 return ServiceResult<List<GetProductDto>>.Fail($"An error occurred while retrieving products: {ex.GetBaseException().Message}");
        //             }
        //         }
        // 
        //         /// <summary>
        //         /// Retrieves a specific product by its ID.
        //         /// </summary>
        //         /// <param name="id">The ID of the product to retrieve.</param>
        //         /// <returns>A ServiceResult containing the GetProductDto for the found product on success,
        //         /// or an error message if not found or on other failures.</returns>
        //         public ServiceResult<GetProductDto> GetProductById(int id)
        //         {
        //             if (id <= 0) // Basic ID validation.
        //                 return ServiceResult<GetProductDto>.Fail("Invalid Product ID.");
        //             try
        //             {
        //                 var productEntity = _productServiceModel.SelectById(id);
        //                 if (productEntity == null)
        //                     // Product with the given ID was not found in the database.
        //                     return ServiceResult<GetProductDto>.Fail("Product not found.");
        // 
        //                 // Map the found entity to a DTO and return a successful result.
        //                 return ServiceResult<GetProductDto>.Success(MapEntityToGetProductDto(productEntity));
        //             }
        //             catch (Exception ex)
        //             {
        //                 Console.WriteLine($"Error in ProductService.GetProductById: {ex.Message}");
        //                 return ServiceResult<GetProductDto>.Fail($"An error occurred while retrieving product details: {ex.GetBaseException().Message}");
        //             }
        //         }
        // 
        //         /// <summary>
        //         /// Updates an existing product.
        //         /// </summary>
        //         /// <param name="id">The ID of the product to be updated.</param>
        //         /// <param name="updateDto">The DTO containing the new values for the product.</param>
        //         /// <returns>A ServiceResult indicating the success or failure of the update operation.</returns>
        //         public ServiceResult UpdateProduct(int id, UpdateProductDto updateDto)
        //         {
        //             // --- Input Validation ---
        //             if (id <= 0)
        //                 return ServiceResult.Fail("Invalid Product ID for update.");
        //             if (updateDto == null)
        //                 return ServiceResult.Fail("Update data cannot be null.");
        //             if (string.IsNullOrWhiteSpace(updateDto.Title))
        //                 return ServiceResult.Fail("Product title is required for update.");
        //             if (updateDto.UnitPrice < 0)
        //                 return ServiceResult.Fail("Unit price cannot be negative for update.");
        //             if (updateDto.Quantity < 0)
        //                 return ServiceResult.Fail("Quantity cannot be negative for update.");
        // 
        //             try
        //             {
        //                 // Map the DTO and ID to a Product entity for the update operation.
        //                 var productToUpdate = MapUpdateDtoToEntity(id, updateDto);
        // 
        //                 // Call the data access layer to perform the update.
        //                 // ProductServiceModel.Update returns true if found and updated, false if not found.
        //                 bool success = _productServiceModel.Update(productToUpdate);
        // 
        //                 if (success)
        //                     return ServiceResult.Success("Product updated successfully.");
        //                 else
        //                     // If success is false, it means ProductServiceModel.Update couldn't find the product.
        //                     return ServiceResult.Fail("Product not found or update failed at data access level.");
        //             }
        //             catch (Exception ex)
        //             {
        //                 Console.WriteLine($"Error in ProductService.UpdateProduct: {ex.Message}");
        //                 return ServiceResult.Fail($"An error occurred while updating the product: {ex.GetBaseException().Message}");
        //             }
        //         }
        // 
        //         /// <summary>
        //         /// Deletes a product by its ID.
        //         /// </summary>
        //         /// <param name="id">The ID of the product to be deleted.</param>
        //         /// <returns>A ServiceResult indicating the success or failure of the delete operation.</returns>
        //         public ServiceResult DeleteProduct(int id)
        //         {
        //             if (id <= 0) // Basic ID validation.
        //                 return ServiceResult.Fail("Invalid Product ID for deletion.");
        //             try
        //             {
        //                 // Call the data access layer to perform the deletion.
        //                 // ProductServiceModel.Delete returns true if found and deleted, false if not found.
        //                 bool success = _productServiceModel.Delete(id);
        //                 if (success)
        //                     return ServiceResult.Success("Product deleted successfully.");
        //                 else
        //                     return ServiceResult.Fail("Product not found or delete failed at data access level.");
        //             }
        //             catch (Exception ex)
        //             {
        //                 Console.WriteLine($"Error in ProductService.DeleteProduct: {ex.Message}");
        //                 return ServiceResult.Fail($"An error occurred while deleting the product: {ex.GetBaseException().Message}");
        //             }
        //         }
        //     }
        // }
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