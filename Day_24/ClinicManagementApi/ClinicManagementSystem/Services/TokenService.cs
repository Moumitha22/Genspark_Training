using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ClinicManagementSystem.Interfaces;
using ClinicManagementSystem.Models;
using Microsoft.IdentityModel.Tokens;

namespace ClinicManagementSystem.Services
{
    // JWT token 
    //  -> Header (algo, type) 
    //  -> Payload (sub, role, exp)
    //  -> Signature( Hash(encode(header) + encode(payload)) using secret key)
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _securityKey;
        public TokenService(IConfiguration configuration)
        {
            _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Keys:JwtTokenKey"]));
        }
        public async Task<string> GenerateToken(User user)
        {
            // pieces of user identity embedded within token
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.Username),
                new Claim(ClaimTypes.Role,user.Role)

            };
            // to create a digital signature using a secret key and a hashing algorithm
            var creds = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256Signature);

            // template for JWT
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };
            var tokenHandler = new JwtSecurityTokenHandler(); // utility to generate, write or validte JWT tokens

            var token = tokenHandler.CreateToken(tokenDescriptor); // returns token object
            return tokenHandler.WriteToken(token); // to return JWT string
        }
    }
}