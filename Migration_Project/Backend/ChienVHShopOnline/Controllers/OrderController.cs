using ChienVHShopOnline.Helpers;
using ChienVHShopOnline.Interfaces;
using ChienVHShopOnline.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ChienVHShopOnline.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;

        public OrderController(IOrderService orderService, ICartService cartService)
        {
            _orderService = orderService;
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderResponseDto>>> GetAll()
        {
            var orders = await _orderService.GetAllAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResponseDto>> GetById(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound();
            return Ok(order);
        }


        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var orders = await _orderService.GetByUserIdAsync(userId);
            return Ok(orders);
        }

        [HttpPost]
        public async Task<ActionResult<OrderResponseDto>> Create([FromBody] OrderRequestDto dto)
        {
            var result = await _orderService.CreateAsync(dto);
            _cartService.ClearCart(HttpContext);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _orderService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpPut("{id}/address")]
        public async Task<IActionResult> UpdateOrderAddress(int id, [FromBody] OrderAddressUpdateDto dto)
        {
            var result = await _orderService.UpdateOrderAddressAsync(id, dto);
            if (!result) return NotFound("Order not found.");
            return NoContent();
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] string newStatus)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, newStatus);
            if (!result) return NotFound("Order not found.");
            return NoContent();
        }

        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var result = await _orderService.CancelOrderAsync(id);
            if (!result) return NotFound("Order not found or already cancelled.");
            return NoContent();
        }

        [HttpGet("export/{id}")]
        public async Task<IActionResult> ExportOrderPdf(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            var pdfBytes = OrderPdfGenerator.Generate(order);
            var fileName = $"Order_{id}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }

    }
}
