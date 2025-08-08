namespace OnlineAuctionAPI.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, string? pdfFilePath = null);
}
