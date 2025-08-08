using ChienVHShopOnline.Models;
using ChienVHShopOnline.Models.Enums;

namespace ChienVHShopOnline.Interfaces
{
    public interface IUserRepository : IRepository<int, User>
    {
        Task<User?> GetByEmailAsync(string email);
        
    }

}