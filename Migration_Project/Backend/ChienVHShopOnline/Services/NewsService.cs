using AutoMapper;
using ChienVHShopOnline.Interfaces;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Models.DTOs;
using ClosedXML.Excel;

namespace ChienVHShopOnline.Services
{
    public class NewsService : INewsService
    {
        private readonly IRepository<int, News> _repo;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;

        public NewsService(IRepository<int, News> repo, IMapper mapper, IWebHostEnvironment environment)
        {
            _repo = repo;
            _mapper = mapper;
            _environment = environment;
        }

        public async Task<IEnumerable<NewsResponseDto>> GetAllNews()
        {
            var news = await _repo.GetAll();
            return _mapper.Map<IEnumerable<NewsResponseDto>>(news);
        }

        public async Task<NewsResponseDto> GetNewsById(int id)
        {
            var news = await _repo.Get(id);
            return _mapper.Map<NewsResponseDto>(news);
        }

        public async Task<NewsResponseDto> CreateNews(NewsRequestDto dto)
        {
            string? fileName = null;

            if (dto.Image != null)
            {
                fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}";
                var imageFolder = Path.Combine(_environment.WebRootPath, "images", "news");

                if (!Directory.Exists(imageFolder))
                    Directory.CreateDirectory(imageFolder);

                var path = Path.Combine(imageFolder, fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await dto.Image.CopyToAsync(stream);
                }
            }

            var news = _mapper.Map<News>(dto);
            news.Image = fileName != null ? $"images/news/{fileName}" : null;
            news.CreatedDate = DateTime.UtcNow;

            var added = await _repo.Add(news);
            return _mapper.Map<NewsResponseDto>(added);
        }


        public async Task<NewsResponseDto> UpdateNews(int id, NewsRequestDto dto)
        {
            var existing = await _repo.Get(id);
            if (existing == null)
                throw new KeyNotFoundException($"News item with ID {id} not found.");

            if (dto.Image != null)
            {
                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}";
                var imageFolder = Path.Combine(_environment.WebRootPath, "images", "news");

                if (!Directory.Exists(imageFolder))
                    Directory.CreateDirectory(imageFolder);

                var path = Path.Combine(imageFolder, fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await dto.Image.CopyToAsync(stream);
                }

                if (!string.IsNullOrEmpty(existing.Image))
                {
                    var oldPath = Path.Combine(_environment.WebRootPath, existing.Image);
                    if (File.Exists(oldPath)) File.Delete(oldPath);
                }

                existing.Image = $"images/news/{fileName}";
            }

            _mapper.Map(dto, existing);
            var updated = await _repo.Update(id, existing);

            return _mapper.Map<NewsResponseDto>(updated);
        }

        public async Task<NewsResponseDto> DeleteNews(int id)
        {
            var existing = await _repo.Get(id);
            if (existing == null)
                throw new KeyNotFoundException($"News with ID {id} not found.");

            if (!string.IsNullOrEmpty(existing.Image))
            {
                var imagePath = Path.Combine(_environment.WebRootPath, existing.Image);
                if (File.Exists(imagePath))
                    File.Delete(imagePath);
            }

            var deleted = await _repo.Delete(id);
            return _mapper.Map<NewsResponseDto>(deleted);
        }

        public async Task<byte[]> ExportToCsv()
        {
            var newsList = await _repo.GetAll();
            var csvBuilder = new StringWriter();
            csvBuilder.WriteLine("NewsId,Title,ShortDescription,CreatedDate,Status");

            foreach (var news in newsList)
            {
                csvBuilder.WriteLine($"\"{news.Id}\",\"{news.Title}\",\"{news.ShortDescription}\",\"{news.CreatedDate}\",\"{news.Status}\"");
            }

            return System.Text.Encoding.UTF8.GetBytes(csvBuilder.ToString());
        }

        public async Task<byte[]> ExportToExcel()
        {
            var newsList = await _repo.GetAll();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("News");
                worksheet.Cell(1, 1).Value = "NewsId";
                worksheet.Cell(1, 2).Value = "Title";
                worksheet.Cell(1, 3).Value = "ShortDescription";
                worksheet.Cell(1, 4).Value = "CreatedDate";
                worksheet.Cell(1, 5).Value = "Status";

                int row = 2;
                foreach (var news in newsList)
                {
                    worksheet.Cell(row, 1).Value = news.Id;
                    worksheet.Cell(row, 2).Value = news.Title;
                    worksheet.Cell(row, 3).Value = news.ShortDescription;
                    worksheet.Cell(row, 4).Value = news.CreatedDate?.ToString("yyyy-MM-dd") ?? "";
                    worksheet.Cell(row, 5).Value = news.Status;
                    row++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

    }
}
