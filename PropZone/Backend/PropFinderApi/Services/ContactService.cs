using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;
using PropFinderApi.Mappers;
using PropFinderApi.Misc;
using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;
using PropFinderApi.Models.DTOs.Responses;
using Microsoft.AspNetCore.SignalR;

namespace PropFinderApi.Services
{
    public class ContactService : IContactLogService
    {
        private readonly IContactLogRepository _contactLogRepository;
        private readonly IUserRepository _userRepository;
        private readonly IListerProfileRepository _listerProfileRepository;
        private readonly IRepository<Guid, Property> _propertyRepository;
        private readonly ContactLogMapper _contactLogMapper;
        private readonly IHubContext<NotificationHub> _hubContext;

        public ContactService(
            IContactLogRepository contactLogRepository,
            IUserRepository userRepository,
            IListerProfileRepository listerProfileRepository,
            IRepository<Guid, Property> propertyRepository,
            IHubContext<NotificationHub> hubContext)
        {
            _contactLogRepository = contactLogRepository;
            _userRepository = userRepository;
            _listerProfileRepository = listerProfileRepository;
            _propertyRepository = propertyRepository;
            _contactLogMapper = new ContactLogMapper();
            _hubContext = hubContext;

        }

        public async Task<ContactListerResponseDto> ContactListerAsync(ContactListerRequestDto requestDto, Guid buyerId)
        {
            if (requestDto.PropertyId == Guid.Empty)
                throw new BadRequestException("Invalid property ID.");

            var buyer = await _userRepository.Get(buyerId);
            await ValidateAndUpdateBuyerAsync(buyer.Id, requestDto.BuyerPhoneNumber);

            var property = await _propertyRepository.Get(requestDto.PropertyId);

            var lister = await _userRepository.Get(property.ListerId);

            var listerProfile = await _listerProfileRepository.GetByUserIdAsync(lister.Id);
            
            var contactLog = new ContactLog
            {
                PropertyId = property.Id,
                BuyerId = buyerId,
                ListerId = lister.Id,
                BuyerPhoneNumber = requestDto.BuyerPhoneNumber,
                BuyerEmail = requestDto.BuyerEmail,
                Message = requestDto.Message,
                ListerName = lister.Name,
                ListerPhoneNumber = listerProfile.BusinessPhoneNumber,
                ListerEmail = lister.Email
            };

            await _contactLogRepository.Add(contactLog);
            await _hubContext.Clients
                .Group(lister.Id.ToString()) 
                .SendAsync("NewInquiryReceived", property.Title, buyer.Name, DateTime.UtcNow.ToString("O"));


            return new ContactListerResponseDto
            {
                ListerName = contactLog.ListerName,
                ListerPhoneNumber = contactLog.ListerPhoneNumber,
                ListerEmail = contactLog.ListerEmail,
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
        
        public async Task<IEnumerable<ListerInquiryDto>> GetContactLogsForPropertyAsync(Guid propertyId, Guid requesterId, string role)
        {
            var property = await _propertyRepository.Get(propertyId);

            if (role == "Lister" && property.ListerId != requesterId)
                throw new UnauthorizedException("You can view only your contact logs");

            var logs = await _contactLogRepository.GetByPropertyIdAsync(propertyId);

            return logs.Select(log => new ListerInquiryDto
            {
                PropertyId = log.PropertyId,
                PropertyTitle = log.Property.Title,
                Location = $"{log.Property.Location.Locality}, {log.Property.Location.City}",
                Message = log.Message,
                CreatedAt = log.CreatedAt,
                BuyerEmail = log.BuyerEmail,
                BuyerPhoneNumber = log.BuyerPhoneNumber
            });
        }
        public async Task<IEnumerable<ListerInquiryDto>> GetContactLogsForListerAsync(Guid listerId, Guid requesterId, string role)
        {
            if (role == "Lister" && listerId != requesterId)
                throw new UnauthorizedException("You can view only your contact logs");
            var logs = await _contactLogRepository.GetByListerIdAsync(listerId);

            return logs.Select(log => new ListerInquiryDto
            {
                PropertyId = log.PropertyId,
                PropertyTitle = log.Property.Title,
                Location = $"{log.Property.Location.Locality}, {log.Property.Location.City}",
                Message = log.Message,
                CreatedAt = log.CreatedAt,
                BuyerEmail = log.BuyerEmail,
                BuyerPhoneNumber = log.BuyerPhoneNumber
            });
        }

        public async Task<IEnumerable<BuyerInquiryDto>> GetContactLogsForBuyerAsync(Guid buyerId, Guid requesterId, string role)
        {
            
            if (role == "Buyer" && buyerId != requesterId)
                throw new UnauthorizedException("You can view only your contact logs");
                   
            var logs = await _contactLogRepository.GetByBuyerIdAsync(buyerId);

            return logs.Select(log => new BuyerInquiryDto
            {
                PropertyId = log.PropertyId,
                PropertyTitle = log.Property.Title,
                Location = $"{log.Property.Location.Locality}, {log.Property.Location.City}",
                Message = log.Message,
                CreatedAt = log.CreatedAt,
                ListerName = log.ListerName,
                ListerEmail = log.ListerEmail,
                ListerPhoneNumber = log.ListerPhoneNumber
            });
        }
    }
}
