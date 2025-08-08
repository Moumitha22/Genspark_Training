using ChienVHShopOnline.Models;

namespace ChienVHShopOnline.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateAccessTokenAsync(User user);
    }
}