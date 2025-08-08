using ChienVHShopOnline.Interfaces;
using ChienVHShopOnline.Models.DTOs;
using ChienVHShopOnline.Helpers;
using System.Text.Json;
using ChienVHShopOnline.Models;

namespace ChienVHShopOnline.Services
{
    public class CaptchaService : ICaptchaService
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;

        public CaptchaService(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> VerifyTokenAsync(string token)
        {
            var secret = _config["Captcha:SecretKey"];
            var client = _httpClientFactory.CreateClient();

            var response = await client.PostAsync(
                $"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={token}",
                null);

            var content = await response.Content.ReadAsStringAsync();
            var captchaResult = JsonSerializer.Deserialize<CaptchaResponse>(content);

            return captchaResult?.Success ?? false;
        }
    }

}