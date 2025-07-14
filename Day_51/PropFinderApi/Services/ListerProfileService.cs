using PropFinderApi.Interfaces;
using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;
using PropFinderApi.Mappers;
using PropFinderApi.Exceptions;

namespace PropFinderApi.Services
{
    public class ListerProfileService : IListerProfileService
    {
        private readonly IListerProfileRepository _listerProfileRepository;
        private readonly IRepository<Guid, User> _userRepository;

        private readonly ListerProfileMapper _listerProfileMapper;

        public ListerProfileService(IListerProfileRepository repository, IRepository<Guid, User> userRepository)
        {
            _listerProfileRepository = repository;
            _userRepository = userRepository;
            _listerProfileMapper = new ListerProfileMapper();
        }

        public async Task<ListerProfile> CreateListerProfileAsync(ListerProfileAddRequestDto agentProfileDto, Guid userId)
        {
            var user = await _userRepository.Get(userId);

            var existing = await GetListerProfileByListerIdAsync(user.Id);

            if (existing != null)
                throw new ConflictException("Lister profile already exists for this user.");

            var agentProfile = _listerProfileMapper.MapListerProfileRequestDtoToListerProfile(agentProfileDto, userId);
            return await _listerProfileRepository.Add(agentProfile);
        }

        public async Task<IEnumerable<ListerProfile>> GetAllAsync()
        {
            return await _listerProfileRepository.GetAll();
        }

        public async Task<ListerProfile> GetByIdAsync(Guid id)
        {
            return await _listerProfileRepository.Get(id);
        }

        public async Task<ListerProfile?> GetListerProfileByListerIdAsync(Guid listerId)
        {
            return await _listerProfileRepository.GetByUserIdAsync(listerId);
        }

        public async Task<ListerProfile?> UpdateListerProfileAsync(Guid profileId, ListerProfileAddRequestDto dto, Guid requesterId, string userRole)
        {
            var profile = await _listerProfileRepository.Get(profileId);

            if (userRole == "Lister" && profile.UserId != requesterId)
                throw new UnauthorizedException("You can only update your own agent profile.");

            profile = _listerProfileMapper.MapUpdatedListerProfile(profile, dto);
            return await _listerProfileRepository.Update(profile.Id, profile);
        }
        
        public async Task<bool> IsProfileCompleteAsync(Guid userId)
        {
            var profile = await _listerProfileRepository.GetByUserIdAsync(userId);

            if (profile == null)
                return false;

            return !string.IsNullOrWhiteSpace(profile.BusinessPhoneNumber);
        }

    }
}
