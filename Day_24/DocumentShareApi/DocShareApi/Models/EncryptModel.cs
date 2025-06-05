namespace DocShareApi.Models
{
    public class EncryptModel
    {
        public string? Data { get; set; }
        public byte[]? HashedData { get; set; }
        public byte[]? HashKey { get; set; }
    }
}