using AutoMapper;
using ChienVHShopOnline.Interfaces;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Models.DTOs;

namespace ChienVHShopOnline.Services
{
    public class ColorService : IColorService
    {
        private readonly IRepository<int, Color> _colorRepository;
        private readonly IMapper _mapper;

        public ColorService(IRepository<int, Color> colorRepository, IMapper mapper)
        {
            _colorRepository = colorRepository;
            _mapper = mapper;
        }

        public async Task<List<ColorResponseDto>> GetAllAsync()
        {
            var colors = await _colorRepository.GetAll();
            return _mapper.Map<List<ColorResponseDto>>(colors);
        }

        public async Task<ColorResponseDto?> GetByIdAsync(int id)
        {
            var color = await _colorRepository.Get(id);
            return color == null ? null : _mapper.Map<ColorResponseDto>(color);
        }

        public async Task<ColorResponseDto> CreateAsync(ColorRequestDto dto)
        {
            var color = _mapper.Map<Color>(dto);
            var created = await _colorRepository.Add(color);
            return _mapper.Map<ColorResponseDto>(created);
        }

        public async Task<bool> UpdateAsync(int id, ColorRequestDto dto)
        {
            var existing = await _colorRepository.Get(id);
            if (existing == null) return false;

            var updatedColor = _mapper.Map(dto, existing); 
            await _colorRepository.Update(id, updatedColor);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _colorRepository.Get(id);
            if (existing == null) return false;

            await _colorRepository.Delete(id);
            return true;
        }
    }
}
