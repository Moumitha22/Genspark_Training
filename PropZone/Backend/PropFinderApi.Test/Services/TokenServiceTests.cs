using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;
using PropFinderApi.Models;
using PropFinderApi.Models.Enums;
using PropFinderApi.Services;

namespace PropFinderApi.Test.Services
{
    [TestFixture]
    public class TokenServiceTests
    {
        private TokenService _tokenService;
        private string _jwtKey = "super_secure_test_key_1234567890";

        [SetUp]
        public void SetUp()
        {
            var inMemorySettings = new Dictionary<string, string> {
                {"Keys:JwtTokenKey", _jwtKey}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _tokenService = new TokenService(configuration);
        }

        [Test]
        public async Task GenerateAccessTokenAsync_ReturnsValidJwtToken_WithCorrectClaims()
        {
            var userId = Guid.Parse("8559c4a0-63e9-4e97-9bf8-af53a5ac2e67");

            var user = new User
            {
                Id = userId,
                Email = "test@example.com",
                Role = UserRole.Lister
            };

            var token = await _tokenService.GenerateAccessTokenAsync(user);

            Assert.IsNotNull(token);

            var handler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey)),
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero
            };

            handler.ValidateToken(token, validationParameters, out var validatedToken);

            var jwtToken = validatedToken as JwtSecurityToken;
            Assert.IsNotNull(jwtToken);


            var nameIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid");
            var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "email");
            var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "role");

            Assert.That(nameIdClaim?.Value, Is.EqualTo(userId.ToString()));
            Assert.That(emailClaim?.Value, Is.EqualTo(user.Email));
            Assert.That(roleClaim?.Value, Is.EqualTo(user.Role.ToString()));
        }
    }
}