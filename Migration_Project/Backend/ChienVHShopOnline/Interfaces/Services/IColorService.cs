using ChienVHShopOnline.Models.DTOs;

namespace ChienVHShopOnline.Interfaces
{
    public interface IColorService
    {
        Task<List<ColorResponseDto>> GetAllAsync();
        Task<ColorResponseDto?> GetByIdAsync(int id);
        Task<ColorResponseDto> CreateAsync(ColorRequestDto dto);
        Task<bool> UpdateAsync(int id, ColorRequestDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
