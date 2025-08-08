using AutoMapper;
using ChienVHShopOnline.Interfaces;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Models.DTOs;
using ChienVHShopOnline.Repositories;

namespace ChienVHShopOnline.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepository,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<List<OrderResponseDto>> GetAllAsync()
        {
            var orders = await _orderRepository.GetAll();
            return _mapper.Map<List<OrderResponseDto>>(orders);
        }

        public async Task<OrderResponseDto?> GetByIdAsync(int id)
        {
            var order = await _orderRepository.Get(id);
            return order == null ? null : _mapper.Map<OrderResponseDto>(order);
        }
        
        public async Task<List<OrderResponseDto>> GetByUserIdAsync(int userId)
        {
            var orders = await _orderRepository.GetOrdersByUserId(userId);
            return _mapper.Map<List<OrderResponseDto>>(orders);
        }

        public async Task<OrderResponseDto> CreateAsync(OrderRequestDto dto)
        {
            var order = _mapper.Map<Order>(dto);
            order.OrderDate = DateTime.UtcNow;
            order.Status = "Placed";

            var added = await _orderRepository.Add(order);
            return _mapper.Map<OrderResponseDto>(added);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _orderRepository.Get(id);
            if (existing == null) return false;

            await _orderRepository.Delete(id);
            return true;
        }
        

        public async Task<bool> CancelOrderAsync(int id)
        {
            var order = await _orderRepository.Get(id);
            if (order == null || order.Status == "Cancelled") return false;

            order.Status = "Cancelled";
            await _orderRepository.Update(id,order);
            return true;
        }

        public async Task<bool> UpdateOrderAddressAsync(int id, OrderAddressUpdateDto dto)
        {
            var order = await _orderRepository.Get(id);
            if (order == null) return false;

            order.CustomerAddress = dto.CustomerAddress;
            order.CustomerPhone = dto.CustomerPhone;
            order.CustomerEmail = dto.CustomerEmail;

            await _orderRepository.Update(id,order);
            return true;
        }

        public async Task<bool> UpdateOrderStatusAsync(int id, string newStatus)
        {
            var order = await _orderRepository.Get(id);
            if (order == null) return false;

            order.Status = newStatus;
            await _orderRepository.Update(id,order);
            return true;
        }
    }
}
