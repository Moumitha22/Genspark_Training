using Microsoft.EntityFrameworkCore;
using OnlineAuctionAPI.Contexts;
using OnlineAuctionAPI.Exceptions;
using OnlineAuctionAPI.Interfaces.Repository;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;

namespace OnlineAuctionAPI.Repositories
{
    public class UserAccountManageRepository : Repository<Guid, User>, IUserAccountManageRepository
    {
        private readonly AuctionContext _context;

        public UserAccountManageRepository(AuctionContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task RestoreUserByEmailAsync(string email)
        {
            try
            {
                var user = await _context
                    .Users.IgnoreQueryFilters()
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                    throw new NotFoundException("User not found");

                if (user.StatusId != 2)
                    throw new InvalidOperationException("User is not deleted");

                var deletedLog = await _context.DeletedUsers.FirstOrDefaultAsync(u =>
                    u.UserId == user.Id
                );
                if (deletedLog != null)
                {
                    _context.DeletedUsers.Remove(deletedLog);
                }

                user.StatusId = 1;
                await Update(user.Id, user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryOperationException("Error restoring user by email.", ex);
            }
        }

        public async Task<string?> GetDeleteReasonByEmailAsync(string email)
        {
            try
            {
                var deletedLog = await _context.DeletedUsers.FirstOrDefaultAsync(d =>
                    d.Email == email
                );

                return deletedLog?.Reason;
            }
            catch (Exception ex)
            {
                throw new RepositoryOperationException(
                    "Error retrieving delete reason by email.",
                    ex
                );
            }
        }

        public async Task SoftDeleteUserAsync(UserDeleteRequest userDeleteDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(userDeleteDto.UserId);
                if (user == null)
                    throw new NotFoundException("User not found");

                if (user.StatusId == 2)
                    throw new AlreadyDeletedException("User is already deleted");

                user.StatusId = 2;
                await Update(user.Id, user);

                await _context.DeletedUsers.AddAsync(
                    new DeletedUsers
                    {
                        UserId = userDeleteDto.UserId,
                        Email = user.Email,
                        Reason = userDeleteDto.Reason,
                        DeletedAt = DateTime.UtcNow,
                    }
                );

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryOperationException("Error soft deleting user.", ex);
            }
        }

        public async Task<List<DeletedUserDto>> GetAllAsync()
        {
            try
            {
                var deletedUsers = await _context
                    .DeletedUsers.Select(d => new DeletedUserDto
                    {
                        Id = d.Id,
                        UserId = d.UserId,
                        Email = d.Email,
                        Reason = d.Reason,
                        DeletedAt = d.DeletedAt,
                    })
                    .ToListAsync();

                return deletedUsers;
            }
            catch (Exception ex)
            {
                throw new RepositoryOperationException("Error retrieving all deleted users.", ex);
            }
        }
    }
}
