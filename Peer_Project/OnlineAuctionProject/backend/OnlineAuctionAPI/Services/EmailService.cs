using System;
using System.Net;
using System.Net.Mail;
using OnlineAuctionAPI.Interfaces;

namespace OnlineAuctionAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(
            string to,
            string subject,
            string body,
            string? pdfFilePath = null
        )
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(
                    _configuration["EmailSettings:SenderEmail"],
                    _configuration["EmailSettings:AppPassword"]
                ),
                EnableSsl = true,
            };

            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            if (string.IsNullOrWhiteSpace(senderEmail))
            {
                throw new InvalidOperationException("Sender email is not configured.");
            }

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(to);

            if (!string.IsNullOrEmpty(pdfFilePath) && File.Exists(pdfFilePath))
            {
                var attachment = new Attachment(pdfFilePath, "application/pdf");
                mailMessage.Attachments.Add(attachment);
            }

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
