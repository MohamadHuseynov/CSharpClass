using Service.DTOs; 


namespace Service
{
    public interface IProductService
    {
        Task<ServiceResult> AddProductAsync(CreateProductDto productDto);
        Task<ServiceResult<List<ProductDto>>> GetAllProductsAsync();
        Task<ServiceResult<ProductDto>> GetProductByIdAsync(int id);
        Task<ServiceResult> UpdateProductAsync(int id, UpdateProductDto productDto);
        Task<ServiceResult> DeleteProductAsync(int id);
    }
}