using PropFinderApi.Interfaces;
using PropFinderApi.Models;
using Microsoft.EntityFrameworkCore;
using PropFinderApi.Models.DTOs;
using PropFinderApi.Mappers;
using PropFinderApi.Exceptions;

namespace PropFinderApi.Services
{
    public class AgentProfileService : IAgentProfileService
    {
        private readonly IAgentProfileRepository _agentProfileRepository;
        private readonly IRepository<Guid, User> _userRepository;

        private readonly AgentProfileMapper _agentProfileMapper;

        public AgentProfileService(IAgentProfileRepository repository, IRepository<Guid, User> userRepository)
        {
            _agentProfileRepository = repository;
            _userRepository = userRepository;
            _agentProfileMapper = new AgentProfileMapper();
        }

        public async Task<AgentProfile> CreateAgentProfileAsync(AgentProfileAddRequestDto agentProfileDto, Guid userId)
        {
            var user = await _userRepository.Get(userId);

            var existing = await GetAgentProfileByAgentIdAsync(user.Id);
            
            if (existing != null)
                throw new ConflictException("Agent profile already exists for this user.");

            var agentProfile = _agentProfileMapper.MapAgentProfileRequestDtoToAgentProfile(agentProfileDto, userId);
            return await _agentProfileRepository.Add(agentProfile);
        }

        public async Task<IEnumerable<AgentProfile>> GetAllAsync()
        {
            return await _agentProfileRepository.GetAll();
        }

        public async Task<AgentProfile> GetByIdAsync(Guid id)
        {
            return await _agentProfileRepository.Get(id);
        }

        public async Task<AgentProfile?> GetAgentProfileByAgentIdAsync(Guid agentId)
        {
            return await _agentProfileRepository.GetByUserIdAsync(agentId);
        }

        public async Task<AgentProfile?> UpdateAgentProfileAsync(Guid profileId, AgentProfileAddRequestDto dto, Guid requesterId, string userRole)
        {
            var profile = await _agentProfileRepository.Get(profileId);

             if (userRole == "Agent" && profile.UserId != requesterId)
                throw new UnauthorizedException("You can only update your own agent profile.");

            profile = _agentProfileMapper.MapUpdatedAgentProfile(profile, dto);
            return await _agentProfileRepository.Update(profile.Id, profile);
        }
    }
}
