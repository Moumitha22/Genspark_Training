using System.Security.Cryptography;
using System.Text;
using DocShareApi.Interfaces;
using DocShareApi.Models;

namespace DocShareApi.Services
{
    public class EncryptionService : IEncryptionService
    {
        public async Task<EncryptModel> HashPassword(EncryptModel data)
        {
            HMACSHA256 hMACSHA256;
            if (data.HashKey != null)
                hMACSHA256 = new HMACSHA256(data.HashKey);
            else
                hMACSHA256 = new HMACSHA256();
            data.HashedData = hMACSHA256.ComputeHash(Encoding.UTF8.GetBytes(data.Data));
            data.HashKey = hMACSHA256.Key;
            return data;
        }
    }
}