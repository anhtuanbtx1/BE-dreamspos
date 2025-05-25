using PosStore.DTOs;

namespace PosStore.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<PagedResult<ProductDto>> GetProductsAsync(PaginationParameters parameters);
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<ProductDto?> GetProductBySkuAsync(string sku);
        Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);
        Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateProductDto);
        Task<bool> DeleteProductAsync(int id);
        Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm);
        Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(string category);
        Task<IEnumerable<ProductStockDto>> GetLowStockProductsAsync(int threshold = 10);
        Task<bool> UpdateProductStockAsync(int productId, int quantity);
        Task<bool> ProductExistsAsync(int id);
        Task<bool> SkuExistsAsync(string sku, int? excludeId = null);
    }
}
