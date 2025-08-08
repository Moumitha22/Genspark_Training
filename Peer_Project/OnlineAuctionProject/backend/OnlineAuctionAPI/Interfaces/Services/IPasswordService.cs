namespace OnlineAuctionAPI.Interfaces;

public interface IPasswordService
{
    string HashPassword(string password);
    bool VerifyPassword(string hashedPassedWord, string password);
}
