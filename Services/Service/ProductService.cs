using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PosStore.Data;
using PosStore.DTOs;
using PosStore.Models;
using PosStore.Services.Interfaces;

namespace PosStore.Services.Service
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProductService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _context.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            return product == null ? null : _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto?> GetProductBySkuAsync(string sku)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.SKU == sku && p.IsActive);

            return product == null ? null : _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
        {
            var product = _mapper.Map<Product>(createProductDto);
            product.CreatedDate = DateTime.UtcNow;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null || !product.IsActive)
                return null;

            _mapper.Map(updateProductDto, product);
            product.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;

            // Soft delete
            product.IsActive = false;
            product.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm)
        {
            var products = await _context.Products
                .Where(p => p.IsActive &&
                    (p.Name.Contains(searchTerm) ||
                     p.SKU.Contains(searchTerm) ||
                     p.Category!.Contains(searchTerm) ||
                     p.Brand!.Contains(searchTerm)))
                .OrderBy(p => p.Name)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(string category)
        {
            var products = await _context.Products
                .Where(p => p.IsActive && p.Category == category)
                .OrderBy(p => p.Name)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductStockDto>> GetLowStockProductsAsync(int threshold = 10)
        {
            var products = await _context.Products
                .Where(p => p.IsActive && p.Quantity <= threshold)
                .Select(p => new ProductStockDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    SKU = p.SKU,
                    Quantity = p.Quantity,
                    IsLowStock = p.Quantity <= threshold
                })
                .OrderBy(p => p.Quantity)
                .ToListAsync();

            return products;
        }

        public async Task<bool> UpdateProductStockAsync(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null || !product.IsActive)
                return false;

            product.Quantity = quantity;
            product.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ProductExistsAsync(int id)
        {
            return await _context.Products.AnyAsync(p => p.Id == id && p.IsActive);
        }

        public async Task<bool> SkuExistsAsync(string sku, int? excludeId = null)
        {
            var query = _context.Products.Where(p => p.SKU == sku && p.IsActive);

            if (excludeId.HasValue)
                query = query.Where(p => p.Id != excludeId.Value);

            return await query.AnyAsync();
        }
    }
}
