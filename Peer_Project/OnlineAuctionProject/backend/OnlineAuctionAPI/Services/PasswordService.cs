using OnlineAuctionAPI.Interfaces;

namespace OnlineAuctionAPI.Service;

public class PasswordService : IPasswordService
{
    public string HashPassword(string password)
    {
        var hasedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        return hasedPassword;
    }

    public bool VerifyPassword(string hashedPassedWord, string password)
    {
        var verifyPasswod = BCrypt.Net.BCrypt.Verify(password, hashedPassedWord);
        return verifyPasswod;
    }
}
