using AutoMapper;
using ChienVHShopOnline.Interfaces;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Models.DTOs;
using ChienVHShopOnline.Repositories;

namespace ChienVHShopOnline.Services
{
    public class StorageService : IStorageService
    {
        private readonly IRepository<int, Storage> _repository;
        private readonly IMapper _mapper;

        public StorageService(IRepository<int, Storage> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StorageResponseDto>> GetAll()
        {
            var storages = await _repository.GetAll();
            return _mapper.Map<IEnumerable<StorageResponseDto>>(storages);
        }

        public async Task<StorageResponseDto> Get(int id)
        {
            var storage = await _repository.Get(id);
            if (storage == null) throw new Exception("Storage not found");

            return _mapper.Map<StorageResponseDto>(storage);
        }

        public async Task<StorageResponseDto> Add(StorageRequestDto dto)
        {
            var storage = _mapper.Map<Storage>(dto);
            var result = await _repository.Add(storage);
            return _mapper.Map<StorageResponseDto>(result);
        }

        public async Task<StorageResponseDto> Update(int id, StorageRequestDto dto)
        {
            var existing = await _repository.Get(id);
            if (existing == null) throw new Exception("Storage not found");

            _mapper.Map(dto, existing); 
            var updated = await _repository.Update(id, existing);

            return _mapper.Map<StorageResponseDto>(updated);
        }

        public async Task<StorageResponseDto> Delete(int id)
        {
            var deleted = await _repository.Delete(id);
            return _mapper.Map<StorageResponseDto>(deleted);
        }
    }
}
