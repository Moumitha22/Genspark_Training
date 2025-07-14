using PropFinderApi.Models;

namespace PropFinderApi.Interfaces
{
    public interface IListerProfileRepository : IRepository<Guid, ListerProfile>
    {
        Task<ListerProfile?> GetByUserIdAsync(Guid userId);
    }
}
