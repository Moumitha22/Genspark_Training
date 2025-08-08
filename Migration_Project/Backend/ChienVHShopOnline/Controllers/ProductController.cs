using ChienVHShopOnline.Interfaces;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChienVHShopOnline.Controllers
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


        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddProduct([FromForm] ProductAddDto dto)
        {
            var result = await _productService.CreateProductAsync(dto);
            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [Authorize]
        [HttpGet("paged")]
        public async Task<ActionResult<IEnumerable<Product>>> GetPagedProducts(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var products = await _productService.GetPagedProductsAsync(pageNumber, pageSize);
            return Ok(products);
        }

        [HttpGet("category/{categoryId}/paged")]
        public async Task<ActionResult<IEnumerable<Product>>> GetPagedProductsByCategory(
             int categoryId,
             [FromQuery] int pageNumber = 1,
             [FromQuery] int pageSize = 10)
        {
            var products = await _productService.GetPagedProductsByCategoryAsync(categoryId, pageNumber, pageSize);
            return Ok(products);
        }

        [HttpGet("user/{userId}/paged")]
        public async Task<IActionResult> GetPagedProductsByUserId(int userId, int pageNumber = 1, int pageSize = 10)
        {
            var products = await _productService.GetPagedProductsByUserIdAsync(userId, pageNumber, pageSize);
            return Ok(products);
        }
        
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductUpdateDto dto)
        {
            if (id != dto.ProductId)
                return BadRequest("ID mismatch");

            var result = await _productService.UpdateProductAsync(dto);
            return Ok(result);
        }

    }
}
