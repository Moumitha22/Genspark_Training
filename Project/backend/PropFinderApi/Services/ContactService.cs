using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;
using PropFinderApi.Mappers;
using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Services
{
    public class ContactService : IContactLogService
    {
        private readonly IContactLogRepository _contactLogRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAgentProfileRepository _agentProfileRepository;
        private readonly IRepository<Guid, Property> _propertyRepository;
        private readonly ContactLogMapper _contactLogMapper;

        public ContactService(
            IContactLogRepository contactLogRepository,
            IUserRepository userRepository,
            IAgentProfileRepository agentProfileRepository,
            IRepository<Guid, Property> propertyRepository)
        {
            _contactLogRepository = contactLogRepository;
            _userRepository = userRepository;
            _agentProfileRepository = agentProfileRepository;
            _propertyRepository = propertyRepository;
            _contactLogMapper = new ContactLogMapper();

        }

        public async Task<ContactAgentResponseDto> ContactAgentAsync(ContactAgentRequestDto requestDto, Guid buyerId)
        {
            var buyer = await ValidateAndUpdateBuyerAsync(buyerId, requestDto.BuyerPhoneNumber);

            var property = await _propertyRepository.Get(requestDto.PropertyId);

            var agent = await _userRepository.GetWithProfileAsync(property.AgentId);

            var contactLog = _contactLogMapper.MapContactAgentRequestDtoToContactLog(requestDto, buyer.Id, property.AgentId);
            await _contactLogRepository.Add(contactLog);

            return new ContactAgentResponseDto
            {
                AgentName = agent.Name,
                AgentEmail = agent.Email,
                AgentPhoneNumber = agent.AgentProfile?.BusinessPhoneNumber ?? "N/A"
            };
        }

        private async Task<User> ValidateAndUpdateBuyerAsync(Guid buyerId, string phoneNumber)
        {
            var buyer = await _userRepository.Get(buyerId);

            if (string.IsNullOrWhiteSpace(buyer.PhoneNumber))
            {
                buyer.PhoneNumber = phoneNumber;
                await _userRepository.Update(buyer.Id, buyer);
            }

            return buyer;
        }

        public async Task<IEnumerable<ContactLog>> GetAllContactLogs()
        {
            return await _contactLogRepository.GetAll();
        }
        public async Task<IEnumerable<ContactLog>> GetContactLogsForAgentAsync(Guid agentId, Guid requesterId, string role)
        {
            if (role == "Agent" && agentId != requesterId)
                throw new UnauthorizedException("You can view only your contact logs");
            return await _contactLogRepository.GetByAgentIdAsync(agentId);
        }

        public async Task<IEnumerable<ContactLog>> GetContactLogsForBuyerAsync(Guid buyerId, Guid requesterId, string role)
        {
            
            if (role == "Buyer" && buyerId != requesterId)
                throw new UnauthorizedException("You can view only your contact logs");
                   
            return await _contactLogRepository.GetByBuyerIdAsync(buyerId);
        }
    }
}
