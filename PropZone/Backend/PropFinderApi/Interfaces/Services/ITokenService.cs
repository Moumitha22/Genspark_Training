using PropFinderApi.Models;

namespace PropFinderApi.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateAccessTokenAsync(User user);
    }
}