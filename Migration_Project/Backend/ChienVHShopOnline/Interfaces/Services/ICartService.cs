using ChienVHShopOnline.Models.DTOs;

namespace ChienVHShopOnline.Interfaces
{
    public interface ICartService
    {
        List<CartItemResponseDto> GetCartItems(HttpContext context);
        Task AddToCart(int currentUserId, HttpContext context, CartItemRequestDto item);
        void UpdateQuantity(HttpContext context, int productId, int quantity);
        void RemoveItem(HttpContext context, int productId);
        void ClearCart(HttpContext context);
    }
}
