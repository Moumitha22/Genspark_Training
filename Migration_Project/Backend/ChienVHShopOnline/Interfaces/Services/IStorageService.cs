using ChienVHShopOnline.Models.DTOs;

namespace ChienVHShopOnline.Interfaces
{
    public interface IStorageService
    {
        Task<IEnumerable<StorageResponseDto>> GetAll();
        Task<StorageResponseDto> Get(int id);
        Task<StorageResponseDto> Add(StorageRequestDto Dto);
        Task<StorageResponseDto> Update(int id, StorageRequestDto Dto);
        Task<StorageResponseDto> Delete(int id);
    }
}
