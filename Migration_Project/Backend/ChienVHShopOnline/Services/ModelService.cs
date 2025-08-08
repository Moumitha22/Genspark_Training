using AutoMapper;
using ChienVHShopOnline.Interfaces;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Models.DTOs;
using ChienVHShopOnline.Repositories;

namespace ChienVHShopOnline.Services
{
    public class ModelService : IModelService
    {
        private readonly IRepository<int, Model> _repository;
        private readonly IMapper _mapper;

        public ModelService(IRepository<int, Model> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ModelResponseDto>> GetAll()
        {
            var models = await _repository.GetAll();
            return _mapper.Map<IEnumerable<ModelResponseDto>>(models);
        }

        public async Task<ModelResponseDto> Get(int id)
        {
            var model = await _repository.Get(id);
            if (model == null) throw new Exception("Model not found");

            return _mapper.Map<ModelResponseDto>(model);
        }

        public async Task<ModelResponseDto> Add(ModelRequestDto dto)
        {
            var model = _mapper.Map<Model>(dto);
            var result = await _repository.Add(model);
            return _mapper.Map<ModelResponseDto>(result);
        }

        public async Task<ModelResponseDto> Update(int id, ModelRequestDto dto)
        {
            var existing = await _repository.Get(id);
            if (existing == null) throw new Exception("Model not found");

            _mapper.Map(dto, existing); // Updates `existing` with values from `dto`
            var updated = await _repository.Update(id, existing);

            return _mapper.Map<ModelResponseDto>(updated);
        }

        public async Task<ModelResponseDto> Delete(int id)
        {
            var deleted = await _repository.Delete(id);
            return _mapper.Map<ModelResponseDto>(deleted);
        }
    }
}
