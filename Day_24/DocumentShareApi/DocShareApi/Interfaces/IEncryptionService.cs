using DocShareApi.Models;

namespace DocShareApi.Interfaces
{
    public interface IEncryptionService
    {
        Task<EncryptModel> HashPassword(EncryptModel data);
    }
}
