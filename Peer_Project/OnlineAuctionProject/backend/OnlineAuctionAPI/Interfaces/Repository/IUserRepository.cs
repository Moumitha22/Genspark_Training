using OnlineAuctionAPI.Interfaces;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;

public interface IUserRepository : IRepository<Guid, User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetUserByRefreshTokenAsync(string refreshToken);
    Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken);
    Task<User?> GetByIdWithVirtualWalletAsync(Guid id);
    Task AddRefreshTokenAsync(RefreshToken refreshToken);
}
