using OnlineAuctionAPI.Interfaces;
using OnlineAuctionAPI.Interfaces.Repository;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;

namespace OnlineAuctionAPI.Services
{
    public class UserAccountManageService : IUserAccountManageService
    {
        private readonly IUserAccountManageRepository _userAccountManageRepository;

        public UserAccountManageService(IUserAccountManageRepository userAccountManageRepository)
        {
            _userAccountManageRepository = userAccountManageRepository;
        }

        public async Task RestoreUserByEmailAsync(string email)
        {
            await _userAccountManageRepository.RestoreUserByEmailAsync(email);
        }

        public async Task SoftDeleteUserAsync(UserDeleteRequest userDeleteDto)
        {
            await _userAccountManageRepository.SoftDeleteUserAsync(userDeleteDto);
        }

        public async Task<string?> GetDeleteReasonByEmail(string email)
        {
            return await _userAccountManageRepository.GetDeleteReasonByEmailAsync(email);
        }

        public async Task<List<DeletedUserDto>> GetAllDeletedUsersAsync()
        {
            return await _userAccountManageRepository.GetAllAsync();
        }
    }
}
