using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;

namespace OnlineAuctionAPI.Interfaces;

public interface IUserService
{
    Task<User> CreateUserAsync(UserRegisterRequestDto userDto);
    Task<User?> GetUserByIdAsync(Guid id);
    Task<User?> GetUserByUsernameAsync(string username);
    Task<IEnumerable<User>> GetAllUsers();
    Task<User?> GetUserByEmailAsync(string email);
    Task<User> UpdateUserInfoAsync(Guid id, UserUpdateRequestDto updateuserdto);
    Task<bool> ChangePasswordAsync(ChangePasswordRequestDto changePasswordDto);
    Task<bool> ForgetPasswordAsync(ForgetPasswordRequestDto forgetPasswordDto);
    Task<User> GetUserWithWalletByUserIdAsync(Guid virtualWalletId);

    Task<IEnumerable<User>> SearchUsersAsync(UserSearchDto searchDto);
    Task<User> CreateAdminAsync(UserRegisterRequestDto userDto);
}
