using ClinicManagementSystem.Models;

namespace ClinicManagementSystem.Interfaces
{
    public interface ITokenService
    {
        public Task<string> GenerateToken(User user);
    }
}