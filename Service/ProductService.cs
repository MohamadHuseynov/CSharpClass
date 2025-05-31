// Namespace for service layer classes, which act as intermediaries.
using Model.DomainModels;    // To use the Product domain entity.
using Model.ServiceModels;   // To use ProductServiceModel for data access.
using Service.DTOs;          // To use Product DTOs (Post, Get, Update) and ServiceResult.
using System;                // For Exception (though try-catch is removed from public methods) and Console.WriteLine (if used for non-exception logging).
using System.Collections.Generic; // For List<T>.
using System.Linq;                // For LINQ methods like .Select().

namespace Service
{
    /// <summary>
    /// Service layer for managing product-related operations.
    /// This service orchestrates data flow between the View layer (e.g., frmProduct)
    /// and the data access component (ProductServiceModel). It handles DTO mapping,
    /// input validation, and wraps results in ServiceResult objects.
    /// Note: In this version, try-catch blocks have been removed from public methods; 
    /// exceptions from ProductServiceModel will propagate to the caller (typically the View layer).
    /// </summary>
    public class ProductService
    {
        // Instance of ProductServiceModel to handle direct database interactions for Product entities.
        // Marked readonly as it's initialized in the constructor and not changed afterwards.
        private readonly ProductServiceModel _productServiceModel;

        /// <summary>
        /// Initializes a new instance of the ProductService class.
        /// In this design, it creates its own instance of ProductServiceModel.
        /// </summary>
        public ProductService()
        {
            _productServiceModel = new ProductServiceModel();
        }

        // --- Private Helper Mapping Methods ---

        /// <summary>
        /// Maps a Product domain entity to a GetProductDto.
        /// </summary>
        /// <param name="product">The Product entity to map from.</param>
        /// <returns>A GetProductDto representation of the product, or null if the input product is null.</returns>
        private GetProductDto MapEntityToGetProductDto(Product product)
        {
            if (product == null) return null; // Guard clause for null input.
            return new GetProductDto
            {
                Id = product.Id,
                Title = product.Title,
                UnitPrice = product.UnitPrice,
                Quantity = product.Quantity
            };
        }

        /// <summary>
        /// Maps a PostProductDto to a new Product domain entity.
        /// </summary>
        /// <param name="postDto">The PostProductDto containing data for the new product.</param>
        /// <returns>A Product entity, or null if the input DTO is null.</returns>
        private Product MapPostDtoToEntity(PostProductDto postDto)
        {
            if (postDto == null) return null;
            return new Product
            {
                Title = postDto.Title.Trim(), // Trim whitespace for consistency.
                UnitPrice = postDto.UnitPrice,
                Quantity = postDto.Quantity
            };
        }

        /// <summary>
        /// Maps an UpdateProductDto and an ID to a Product domain entity, typically for an update operation.
        /// </summary>
        /// <param name="id">The ID of the product being updated.</param>
        /// <param name="updateDto">The UpdateProductDto containing the new values.</param>
        /// <returns>A Product entity with the ID and updated values, or null if the input DTO is null.</returns>
        private Product MapUpdateDtoToEntity(int id, UpdateProductDto updateDto)
        {
            if (updateDto == null) return null;
            return new Product
            {
                Id = id, // The ID is crucial for identifying the entity to update.
                Title = updateDto.Title.Trim(), // Trim whitespace.
                UnitPrice = updateDto.UnitPrice,
                Quantity = updateDto.Quantity
            };
        }

        // --- Public Service Methods ---

        /// <summary>
        /// Adds a new product to the system after validating the input DTO.
        /// Exceptions from the data access layer (_productServiceModel) will propagate to the caller.
        /// </summary>
        /// <param name="postDto">The DTO containing the details of the product to add.</param>
        /// <returns>A ServiceResult containing the GetProductDto of the newly added product on success,
        /// or a failed ServiceResult if input validation fails.</returns>
        public ServiceResult<GetProductDto> AddProduct(PostProductDto postDto)
        {
            // --- Input Validation ---
            if (postDto == null)
                return ServiceResult<GetProductDto>.Fail("Product data cannot be null.");
            if (string.IsNullOrWhiteSpace(postDto.Title))
                return ServiceResult<GetProductDto>.Fail("Product title is required.");
            if (postDto.UnitPrice < 0)
                return ServiceResult<GetProductDto>.Fail("Unit price cannot be negative.");
            if (postDto.Quantity < 0)
                return ServiceResult<GetProductDto>.Fail("Quantity cannot be negative.");

            var productEntity = MapPostDtoToEntity(postDto);
            if (productEntity == null) // Should ideally not happen if postDto passed validation.
                return ServiceResult<GetProductDto>.Fail("Failed to map DTO to entity.");

            _productServiceModel.Post(productEntity); // This might throw an exception.

            var resultDto = MapEntityToGetProductDto(productEntity); // Map entity (now with ID) to DTO.
            return ServiceResult<GetProductDto>.Success(resultDto, "Product added successfully.");
        }

        /// <summary>
        /// Retrieves all products from the system.
        /// Exceptions from the data access layer (_productServiceModel) will propagate to the caller.
        /// </summary>
        /// <returns>A ServiceResult containing a list of GetProductDto on success.</returns>
        public ServiceResult<List<GetProductDto>> GetAllProducts()
        {
            var productEntities = _productServiceModel.SelectAll();
            var productDtos = productEntities.Select(MapEntityToGetProductDto).ToList();
            return ServiceResult<List<GetProductDto>>.Success(productDtos);
        }

        /// <summary>
        /// Retrieves a specific product by its ID.
        /// Exceptions from the data access layer (_productServiceModel) will propagate to the caller.
        /// </summary>
        /// <param name="id">The ID of the product to retrieve.</param>
        /// <returns>A ServiceResult containing the GetProductDto for the found product on success,
        /// or a failed ServiceResult if not found or ID is invalid.</returns>
        public ServiceResult<GetProductDto> GetProductById(int id)
        {
            if (id <= 0)
                return ServiceResult<GetProductDto>.Fail("Invalid Product ID.");

            var productEntity = _productServiceModel.SelectById(id);
            if (productEntity == null)
                return ServiceResult<GetProductDto>.Fail("Product not found.");

            return ServiceResult<GetProductDto>.Success(MapEntityToGetProductDto(productEntity));
        }

        /// <summary>
        /// Updates an existing product.
        /// Exceptions from the data access layer (_productServiceModel) will propagate to the caller.
        /// </summary>
        /// <param name="id">The ID of the product to be updated.</param>
        /// <param name="updateDto">The DTO containing the new values for the product.</param>
        /// <returns>A ServiceResult indicating the success or failure of the update operation.</returns>
        public ServiceResult UpdateProduct(int id, UpdateProductDto updateDto)
        {
            // --- Input Validation ---
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

            var productToUpdate = MapUpdateDtoToEntity(id, updateDto);
            if (productToUpdate == null) // Should ideally not happen if updateDto passed validation.
                return ServiceResult.Fail("Failed to map DTO to entity for update.");

            bool success = _productServiceModel.Update(productToUpdate);

            if (success)
                return ServiceResult.Success("Product updated successfully.");
            else
                return ServiceResult.Fail("Product not found or update failed at data access level.");
        }

        /// <summary>
        /// Deletes a product by its ID.
        /// Exceptions from the data access layer (_productServiceModel) will propagate to the caller.
        /// </summary>
        /// <param name="id">The ID of the product to be deleted.</param>
        /// <returns>A ServiceResult indicating the success or failure of the delete operation.</returns>
        public ServiceResult DeleteProduct(int id)
        {
            if (id <= 0)
                return ServiceResult.Fail("Invalid Product ID for deletion.");

            bool success = _productServiceModel.Delete(id);
            if (success)
                return ServiceResult.Success("Product deleted successfully.");
            else
                return ServiceResult.Fail("Product not found or delete failed at data access level.");
        }
    }
}