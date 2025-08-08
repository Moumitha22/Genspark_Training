using ChienVHShopOnline.Interfaces;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Models.DTOs;
using ChienVHShopOnline.Helpers;
using System.Threading.Tasks;

namespace ChienVHShopOnline.Services
{
    public class CartService : ICartService
    {
        private const string CartSessionKey = "Cart";
        private readonly IRepository<int, Product> _productRepository;

        public CartService(IRepository<int, Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public List<CartItemResponseDto> GetCartItems(HttpContext context)
        {
            return context.Session.GetObject<List<CartItemResponseDto>>(CartSessionKey) ?? new();
        }

        public async Task AddToCart(int currentUserId, HttpContext context, CartItemRequestDto request)
        {
            var cart = GetCartItems(context);
            var existingItem = cart.FirstOrDefault(c => c.ProductId == request.ProductId);

            var product = await _productRepository.Get(request.ProductId);
            if (product == null)
                throw new Exception("Product not found");

            if (product.UserId == currentUserId)
                throw new Exception("You cannot add your own product to the cart");

            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
            }
            else
            {
                cart.Add(new CartItemResponseDto
                {
                    ProductId = product.Id,
                    ProductName = product.ProductName,
                    UnitPrice = product.Price,
                    Quantity = request.Quantity
                });
            }

            context.Session.SetObject(CartSessionKey, cart);
        }


        public void UpdateQuantity(HttpContext context, int productId, int quantity)
        {
            var cart = GetCartItems(context);
            var existingItem = cart.FirstOrDefault(c => c.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity = quantity;
                context.Session.SetObject(CartSessionKey, cart);
            }
        }

        public void RemoveItem(HttpContext context, int productId)
        {
            var cart = GetCartItems(context);
            var itemToRemove = cart.FirstOrDefault(c => c.ProductId == productId);
            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
                context.Session.SetObject(CartSessionKey, cart);
            }
        }

        public void ClearCart(HttpContext context)
        {
            context.Session.Remove(CartSessionKey);
        }
    }
}
