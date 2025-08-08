using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;

namespace OnlineAuctionAPI.Interfaces.Repository
{
    public interface IUserAccountManageRepository
    {
        Task SoftDeleteUserAsync(UserDeleteRequest userDeleteDto);
        Task<string?> GetDeleteReasonByEmailAsync(string email);
        Task RestoreUserByEmailAsync(string email);
        Task<List<DeletedUserDto>> GetAllAsync();
    }
}
