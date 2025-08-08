using ChienVHShopOnline.Models.DTOs;

namespace ChienVHShopOnline.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryResponseDto>> GetAllAsync();
        Task<CategoryResponseDto?> GetByIdAsync(int id);
        Task<CategoryResponseDto> CreateAsync(CategoryRequestDto dto);
        Task<bool> UpdateAsync(int id, CategoryRequestDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
