using AutoMapper;
using ChienVHShopOnline.Interfaces;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Models.DTOs;

namespace ChienVHShopOnline.Services
{
    public class ContactUsService : IContactUsService
    {
        private readonly IRepository<int, ContactUs> _repository;
        private readonly IMapper _mapper;

        public ContactUsService(IRepository<int, ContactUs> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task SubmitAsync(ContactUsRequestDto dto)
        {
            var entity = _mapper.Map<ContactUs>(dto);
            await _repository.Add(entity);
        }

        public async Task<IEnumerable<ContactUsResponseDto>> GetAllAsync()
        {
            var list = await _repository.GetAll();
            return _mapper.Map<IEnumerable<ContactUsResponseDto>>(list);
        }

        public async Task<ContactUsResponseDto> GetByIdAsync(int id)
        {
            var entity = await _repository.Get(id);
            return _mapper.Map<ContactUsResponseDto>(entity);
        }
    }
}
