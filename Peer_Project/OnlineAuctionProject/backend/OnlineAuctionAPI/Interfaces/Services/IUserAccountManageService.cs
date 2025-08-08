using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;

namespace OnlineAuctionAPI.Interfaces;

public interface IUserAccountManageService
{
    Task RestoreUserByEmailAsync(string email);
    Task SoftDeleteUserAsync(UserDeleteRequest userDeleteDto);
    Task<string?> GetDeleteReasonByEmail(string email);
    Task<List<DeletedUserDto>> GetAllDeletedUsersAsync();
}
