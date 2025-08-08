using ChienVHShopOnline.Models.DTOs;

namespace ChienVHShopOnline.Interfaces
{
    public interface IModelService
    {
        Task<IEnumerable<ModelResponseDto>> GetAll();
        Task<ModelResponseDto> Get(int id);
        Task<ModelResponseDto> Add(ModelRequestDto Dto);
        Task<ModelResponseDto> Update(int id, ModelRequestDto Dto);
        Task<ModelResponseDto> Delete(int id);
    }
}
