using Microsoft.AspNetCore.Mvc;
using PosStore.DTOs;
using PosStore.Services.Interfaces;

namespace PosStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<ProductDto>>> GetProducts([FromQuery] PaginationParameters parameters)
        {
            var result = await _productService.GetProductsAsync(parameters);
            return Ok(result);
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto dto)
        {
            var product = await _productService.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
    }
}
