using ChienVHShopOnline.Models.DTOs;

namespace ChienVHShopOnline.Interfaces
{
    public interface IContactUsService
    {
        Task SubmitAsync(ContactUsRequestDto dto);
        Task<IEnumerable<ContactUsResponseDto>> GetAllAsync();
        Task<ContactUsResponseDto> GetByIdAsync(int id);
    }
}
