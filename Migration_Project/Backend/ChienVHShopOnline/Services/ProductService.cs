using AutoMapper;
using ChienVHShopOnline.Interfaces;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Models.DTOs;

namespace ChienVHShopOnline.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepo, IWebHostEnvironment environment, IMapper mapper)
        {
            _productRepo = productRepo;
            _environment = environment;
            _mapper = mapper;
        }

        public async Task<ProductResponseDto> CreateProductAsync(ProductAddDto dto)
        {
            string fileName = null;
            if (dto.Image != null)
            {
                fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}";

                var imageFolder = Path.Combine(_environment.WebRootPath, "images", "products");
                if (!Directory.Exists(imageFolder))
                    Directory.CreateDirectory(imageFolder);

                var path = Path.Combine(imageFolder, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await dto.Image.CopyToAsync(stream);
                }
            }
            var product = _mapper.Map<Product>(dto);
            product.Image = fileName != null ? $"images/products/{fileName}" : null;

            product.SellStartDate = dto.SellStartDate?.ToUniversalTime();
            product.SellEndDate = dto.SellEndDate?.ToUniversalTime();

            var added = await _productRepo.Add(product);

            return _mapper.Map<ProductResponseDto>(added);
        }

        public async Task<ProductResponseDto> GetProductByIdAsync(int id)
        {
            var product = await _productRepo.Get(id);
            return _mapper.Map<ProductResponseDto>(product);
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync()
        {
            var products = await _productRepo.GetAll();
            return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
        }

        public async Task<IEnumerable<ProductResponseDto>> GetPagedProductsAsync(int pageNumber, int pageSize)
        {
            var products = await _productRepo.GetPagedProducts(pageNumber, pageSize);
            return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
        }

        public async Task<IEnumerable<ProductResponseDto>> GetPagedProductsByCategoryAsync(int categoryId, int pageNumber, int pageSize)
        {
            var products = await _productRepo.GetPagedProductsByCategory(categoryId, pageNumber, pageSize);
            return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
        }

        public async Task<IEnumerable<ProductResponseDto>> GetPagedProductsByUserIdAsync(int userId, int pageNumber, int pageSize)
        {
            var products = await _productRepo.GetPagedProductsByUserId(userId, pageNumber, pageSize);
            return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
        }


        public async Task<ProductResponseDto> UpdateProductAsync(ProductUpdateDto dto)
        {
            var existing = await _productRepo.Get(dto.ProductId);

            if (existing == null)
                throw new KeyNotFoundException($"Product with ID {dto.ProductId} not found.");

            if (dto.Image != null)
            {
                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}";
                var imageFolder = Path.Combine(_environment.WebRootPath, "images", "products");

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

                existing.Image = $"images/products/{fileName}";
            }

            _mapper.Map(dto, existing);

            existing.SellStartDate = dto.SellStartDate?.ToUniversalTime();
            existing.SellEndDate = dto.SellEndDate?.ToUniversalTime();

            await _productRepo.Update(dto.ProductId, existing);

            return _mapper.Map<ProductResponseDto>(existing);
        }
        
        public async Task<ProductResponseDto> DeleteProduct(int id)
        {
            var deleted = await _productRepo.Delete(id);
            return _mapper.Map<ProductResponseDto>(deleted);
        }

    }
}
