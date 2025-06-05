// using System.Security.Claims;
// using Microsoft.AspNetCore.Authentication;
// using DocShareApi.Interfaces;
// using DocShareApi.Models;
// using DocShareApi.Models.DTOs;

// namespace DocShareApi.Services
// {
//     public class GoogleAuthService : IGoogleAuthService
//     {
//         private readonly ITokenService _tokenService;

//         public GoogleAuthService(ITokenService tokenService)
//         {
//             _tokenService = tokenService;
//         }

//         public async Task<LoginResponseDto> AuthenticateUser(AuthenticateResult result)
//         {
//             var name = result.Principal?.FindFirst(ClaimTypes.Name)?.Value;
//             var email = result.Principal?.FindFirst(ClaimTypes.Email)?.Value;

//             var user = new User
//             {
//                 Email = email ?? string.Empty,
//                 Name = name ?? string.Empty,
//                 Role = "HRAdmin",
//                 Status = "Active"
//             };

//             var token = await _tokenService.GenerateToken(user);

//             return new LoginResponseDto
//             {
//                 Username = user.Name,
//                 Token = token
//             };
//         }
//     }
// }
