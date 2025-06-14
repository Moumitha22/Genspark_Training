namespace PropFinderApi.Interfaces
{
    public interface IEncryptionService
    {
        string HashPassword(string plainPassword);
        bool VerifyPassword(string plainPassword, string hashedPassword);
    }
}
