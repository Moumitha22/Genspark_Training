using System.Security.Claims;
using ChienVHShopOnline.Interfaces;
using ChienVHShopOnline.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ChienVHShopOnline.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public IActionResult GetCart()
        {
            var cartItems = _cartService.GetCartItems(HttpContext);
            return Ok(cartItems);
        }

        // [HttpPost]
        // public async Task<IActionResult> AddToCart([FromBody] CartItemRequestDto item)
        // {
        //     if (item == null || item.ProductId <= 0 || item.Quantity <= 0)
        //     {
        //         return BadRequest(new { Message = "Invalid cart item" });
        //     }

        //     var userId = GetUserId(); 
        //     await _cartService.AddToCart(userId, HttpContext, item); 


        //     var updatedCart = _cartService.GetCartItems(HttpContext);
        //     return Ok(new { message = "Added to Cart", cartCount = updatedCart.Count });
        // }
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] CartItemRequestDto item)
        {
            if (item == null || item.ProductId <= 0 || item.Quantity <= 0)
            {
                return BadRequest(new { message = "Invalid cart item" });
            }

            try
            {
                var userId = GetUserId();
                await _cartService.AddToCart(userId, HttpContext, item);

                var updatedCart = _cartService.GetCartItems(HttpContext);
                return Ok(new { message = "Added to Cart", cartCount = updatedCart.Count });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPut("{productId}")]
        public IActionResult UpdateQuantity(int productId, [FromBody] int quantity)
        {
            if (quantity <= 0)
            {
                return BadRequest(new { Message = "Quantity must be greater than 0" });
            }

            var cartItems = _cartService.GetCartItems(HttpContext);
            var existing = cartItems.Any(c => c.ProductId == productId);
            if (!existing)
            {
                return NotFound(new { Message = "Item not found in cart" });
            }

            _cartService.UpdateQuantity(HttpContext, productId, quantity);
            return Ok(new { Message = "Quantity updated" });
        }

        [HttpDelete("{productId}")]
        public IActionResult RemoveItem(int productId)
        {
            _cartService.RemoveItem(HttpContext, productId);
            return Ok(new { Message = "Item removed from cart" });
        }

        [HttpDelete]
        public IActionResult ClearCart()
        {
            _cartService.ClearCart(HttpContext);
            return Ok(new { Message = "Cart cleared" });
        }

        [HttpGet("total")]
        public IActionResult GetCartTotal()
        {
            var cart = _cartService.GetCartItems(HttpContext);
            var total = cart.Sum(c => (c.UnitPrice ?? 0) * c.Quantity);
            return Ok(new { total });
        }

        [HttpGet("count")]
        public IActionResult GetCartCount()
        {
            var cartItems = _cartService.GetCartItems(HttpContext);
            return Ok(new { count = cartItems.Count });
        }

        private int GetUserId()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdValue, out var userId) ? userId : 0;
        }
    }
}
