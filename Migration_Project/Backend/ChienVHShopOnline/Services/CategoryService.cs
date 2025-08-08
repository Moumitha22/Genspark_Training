// using ChienVHShopOnline.Models.DTOs;
// using ChienVHShopOnline.Interfaces;
// using ChienVHShopOnline.Models;
// using ChienVHShopOnline.Repositories;

// namespace ChienVHShopOnline.Services
// {
//     public class CategoryService : ICategoryService
//     {
//         private readonly IRepository<int, Category> _categoryRepository;

//         public CategoryService(IRepository<int, Category> categoryRepository)
//         {
//             _categoryRepository = categoryRepository;
//         }

//         public async Task<CategoryResponseDto> CreateAsync(CategoryRequestDto dto)
//         {
//             var category = new Category
//             {
//                 Name = dto.Name
//             };

//             var result = await _categoryRepository.Add(category);
//             return new CategoryResponseDto
//             {
//                 Id = result.Id,
//                 Name = result.Name
//             };
//         }

//         public async Task<List<CategoryResponseDto>> GetAllAsync()
//         {
//             var categories = await _categoryRepository.GetAll();
//             return categories.Select(c => new CategoryResponseDto
//             {
//                 Id = c.Id,
//                 Name = c.Name
//             }).ToList();
//         }

//         public async Task<CategoryResponseDto?> GetByIdAsync(int id)
//         {
//             var category = await _categoryRepository.Get(id);
//             if (category == null)
//                 return null;

//             return new CategoryResponseDto
//             {
//                 Id = category.Id,
//                 Name = category.Name
//             };
//         }

//         public async Task<bool> UpdateAsync(int id, CategoryRequestDto dto)
//         {
//             var category = new Category
//             {
//                 Id = id,
//                 Name = dto.Name
//             };

//             try
//             {
//                 await _categoryRepository.Update(id, category);
//                 return true;
//             }
//             catch
//             {
//                 return false;
//             }
//         }

//         public async Task<bool> DeleteAsync(int id)
//         {
//             var category = await _categoryRepository.Delete(id);
//             return category != null;
//         }
//     }
// }


using AutoMapper;
using ChienVHShopOnline.Models.DTOs;
using ChienVHShopOnline.Interfaces;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Repositories;

namespace ChienVHShopOnline.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<int, Category> _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(IRepository<int, Category> categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<CategoryResponseDto> CreateAsync(CategoryRequestDto dto)
        {
            var category = _mapper.Map<Category>(dto);
            var result = await _categoryRepository.Add(category);
            return _mapper.Map<CategoryResponseDto>(result);
        }

        public async Task<List<CategoryResponseDto>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAll();
            return _mapper.Map<List<CategoryResponseDto>>(categories);
        }

        public async Task<CategoryResponseDto?> GetByIdAsync(int id)
        {
            var category = await _categoryRepository.Get(id);
            if (category == null)
                return null;

            return _mapper.Map<CategoryResponseDto>(category);
        }

        public async Task<bool> UpdateAsync(int id, CategoryRequestDto dto)
        {
            var category = _mapper.Map<Category>(dto);
            category.Id = id;

            try
            {
                await _categoryRepository.Update(id, category);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _categoryRepository.Delete(id);
            return category != null;
        }
    }
}
