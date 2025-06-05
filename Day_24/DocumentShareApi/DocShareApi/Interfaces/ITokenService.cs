using DocShareApi.Models;

namespace DocShareApi.Interfaces
{
    public interface ITokenService
    {
        public Task<string> GenerateToken(User user);
    }
}