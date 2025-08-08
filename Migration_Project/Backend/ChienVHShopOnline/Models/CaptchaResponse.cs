using System.Text.Json.Serialization;

namespace ChienVHShopOnline.Models
{
    public class CaptchaResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("error-codes")]
        public List<string> ErrorCodes { get; set; }
    }
    
}