using PropFinderApi.Models;

namespace PropFinderApi.Interfaces
{
    public interface IUserRepository : IRepository<Guid, User>
    {
        Task<User?> GetByEmailAsync(string email);

        Task<User?> GetWithProfileAsync(Guid id);

    }

}