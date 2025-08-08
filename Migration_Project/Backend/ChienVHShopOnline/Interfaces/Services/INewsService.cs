using ChienVHShopOnline.Models.DTOs;

namespace ChienVHShopOnline.Services
{
    public interface INewsService
    {
        Task<IEnumerable<NewsResponseDto>> GetAllNews();
        Task<NewsResponseDto> GetNewsById(int id);
        Task<NewsResponseDto> CreateNews(NewsRequestDto dto);
        Task<NewsResponseDto> UpdateNews(int id, NewsRequestDto dto);
        Task<NewsResponseDto> DeleteNews(int id);

        Task<byte[]> ExportToCsv();
        Task<byte[]> ExportToExcel(); 

    }
}
