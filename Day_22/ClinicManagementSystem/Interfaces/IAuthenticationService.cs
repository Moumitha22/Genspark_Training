
using ClinicManagementSystem.Models.DTOs;

namespace ClinicManagementSystem.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<UserLoginResponse> Login(UserLoginRequest user);
    }
}
