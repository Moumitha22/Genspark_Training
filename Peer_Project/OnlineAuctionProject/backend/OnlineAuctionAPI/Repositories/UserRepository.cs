using Microsoft.EntityFrameworkCore;
using OnlineAuctionAPI.Contexts;
using OnlineAuctionAPI.Exceptions;
using OnlineAuctionAPI.Helpers;
using OnlineAuctionAPI.Models;

namespace OnlineAuctionAPI.Repositories;

using OnlineAuctionAPI.Models.DTO;

public class UserRepository : Repository<Guid, User>, IUserRepository
{
    public UserRepository(AuctionContext context)
        : base(context) { }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException(
                $"{nameof(username)} must not be null, empty or whitespace"
            );

        try
        {
            var users = await _auctionContext.Users.FirstOrDefaultAsync(u =>
                u.Username == username
            );
            return users;
        }
        catch (Exception ex)
        {
            throw new RepositoryOperationException("Get users by userName", ex);
        }
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException($"{nameof(email)} must not be null, empty or whitespace");
        try
        {
            var usersByEmail = await _auctionContext.Users.IgnoreQueryFilters().FirstOrDefaultAsync(e =>
                e.Email == email
            );
            return usersByEmail;
        }
        catch (Exception ex)
        {
            throw new RepositoryOperationException("Get users by Email", ex);
        }
    }

    public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
    {
        try
        {
            var user = await _auctionContext
                .Users.Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(r => r.Token == refreshToken));
            return user;
        }
        catch (Exception ex)
        {
            throw new RepositoryOperationException("Get user by refresh token", ex);
        }
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken)
    {
        try
        {
            return await _auctionContext.RefreshTokens.FirstOrDefaultAsync(rt =>
                rt.Token == refreshToken
            );
        }
        catch (Exception ex)
        {
            throw new RepositoryOperationException("Get refresh token", ex);
        }
    }

    public async Task<User?> GetByIdWithVirtualWalletAsync(Guid id)
    {
        try
        {
            var user = await _auctionContext
                .Users.Include(u => u.VirtualWallet)
                .FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }
        catch (Exception ex)
        {
            throw new RepositoryOperationException("Get user by Id with Virtual Wallet", ex);
        }
    }

    public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
    {
        try
        {
            _auctionContext.RefreshTokens.Add(refreshToken);
            await _auctionContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryOperationException("Add refresh token", ex);
        }
    }
}
