using ChienVHShopOnline.Models;
using ChienVHShopOnline.Models.DTOs;

namespace ChienVHShopOnline.Interfaces
{
    public interface IProductService
    {
        Task<ProductResponseDto> CreateProductAsync(ProductAddDto dto);
        Task<ProductResponseDto> GetProductByIdAsync(int id);
        Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync();
        Task<IEnumerable<ProductResponseDto>> GetPagedProductsAsync(int pageNumber, int pageSize);
        Task<IEnumerable<ProductResponseDto>> GetPagedProductsByCategoryAsync(int categoryId, int pageNumber, int pageSize);
        Task<IEnumerable<ProductResponseDto>> GetPagedProductsByUserIdAsync(int userId, int pageNumber, int pageSize);
        Task<ProductResponseDto> UpdateProductAsync(ProductUpdateDto dto);
    }
}
