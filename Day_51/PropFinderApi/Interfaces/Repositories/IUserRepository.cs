using PropFinderApi.Models;
using PropFinderApi.Models.Enums;

namespace PropFinderApi.Interfaces
{
    public interface IUserRepository : IRepository<Guid, User>
    {
        Task<User?> GetByEmailAsync(string email);

        Task<User?> GetByEmailandRoleAsync(string email, UserRole role);

        Task<User?> GetWithProfileAsync(Guid id);

    }

}