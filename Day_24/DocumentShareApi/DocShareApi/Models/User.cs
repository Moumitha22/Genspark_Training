using System.ComponentModel.DataAnnotations;

namespace DocShareApi.Models
{
    public class User
    {
        public string Email { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public byte[] Password { get; set; } = Array.Empty<byte>();
        
        public byte[] HashKey { get; set; } = Array.Empty<byte>();

        public string Role { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public ICollection<Document>? Documents { get; set; }
    }
}
