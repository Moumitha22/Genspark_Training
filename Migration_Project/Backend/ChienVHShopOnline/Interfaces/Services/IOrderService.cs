using ChienVHShopOnline.Models.DTOs;

namespace ChienVHShopOnline.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderResponseDto>> GetAllAsync();
        Task<OrderResponseDto?> GetByIdAsync(int id);
        Task<List<OrderResponseDto>> GetByUserIdAsync(int userId);
        Task<OrderResponseDto> CreateAsync(OrderRequestDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> CancelOrderAsync(int id);
        Task<bool> UpdateOrderAddressAsync(int id, OrderAddressUpdateDto dto);
        Task<bool> UpdateOrderStatusAsync(int id, string newStatus);
    }
}
