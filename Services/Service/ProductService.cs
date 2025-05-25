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

        public async Task<PagedResult<ProductDto>> GetProductsAsync(PaginationParameters parameters)
        {
            var query = _context.Products.Where(p => p.IsActive);

            // Apply search filter
            if (!string.IsNullOrEmpty(parameters.SearchTerm))
            {
                query = query.Where(p =>
                    p.Name.Contains(parameters.SearchTerm) ||
                    p.SKU.Contains(parameters.SearchTerm) ||
                    (p.Category != null && p.Category.Contains(parameters.SearchTerm)) ||
                    (p.Brand != null && p.Brand.Contains(parameters.SearchTerm)));
            }

            // Apply category filter
            if (!string.IsNullOrEmpty(parameters.Category))
            {
                query = query.Where(p => p.Category == parameters.Category);
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(parameters.SortBy))
            {
                switch (parameters.SortBy.ToLower())
                {
                    case "name":
                        query = parameters.SortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(p => p.Name)
                            : query.OrderBy(p => p.Name);
                        break;
                    case "price":
                        query = parameters.SortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(p => p.Price)
                            : query.OrderBy(p => p.Price);
                        break;
                    case "quantity":
                        query = parameters.SortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(p => p.Quantity)
                            : query.OrderBy(p => p.Quantity);
                        break;
                    case "createddate":
                        query = parameters.SortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(p => p.CreatedDate)
                            : query.OrderBy(p => p.CreatedDate);
                        break;
                    default:
                        query = query.OrderBy(p => p.Name);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(p => p.Name);
            }

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply pagination
            var products = await query
                .Skip((parameters.Page - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

            // Calculate pagination info
            var totalPages = (int)Math.Ceiling((double)totalCount / parameters.PageSize);

            return new PagedResult<ProductDto>
            {
                Data = productDtos,
                Pagination = new PaginationInfo
                {
                    CurrentPage = parameters.Page,
                    PageSize = parameters.PageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    HasPrevious = parameters.Page > 1,
                    HasNext = parameters.Page < totalPages
                }
            };
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
