using System.Globalization;
using System.IO;
using OnlineAuctionAPI.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace OnlineAuctionAPI.Helpers
{
    public static class PdfHelper
    {
        public static byte[] GenerateAgreementPdf(AuctionItem auction, BidItem winningBid)
        {
            using var ms = new MemoryStream();
            var indiaCulture = new CultureInfo("en-IN");
            var ist = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            var localStartTime = TimeZoneInfo.ConvertTimeFromUtc(auction.StartTime, ist);
            var localEndTime = TimeZoneInfo.ConvertTimeFromUtc(auction.EndTime, ist);
            var localCreatedAt = TimeZoneInfo.ConvertTimeFromUtc(auction.CreatedAt, ist);
            var localUpdatedAt = TimeZoneInfo.ConvertTimeFromUtc(auction.UpdatedAt, ist);
            var localBidTime = TimeZoneInfo.ConvertTimeFromUtc(winningBid.BidTime, ist);

            Document
                .Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.Background(Colors.White);
                        page.Content()
                            .Column(col =>
                            {
                                col.Item()
                                    .Text("E-Agreement")
                                    .FontSize(24)
                                    .Bold()
                                    .FontColor(Colors.Blue.Medium);

                                col.Item()
                                    .PaddingBottom(5)
                                    .Text("Auction Details")
                                    .FontSize(18)
                                    .Bold()
                                    .FontColor(Colors.Grey.Darken2);

                                col.Item()
                                    .Text($"Auction ID: {auction.Id}")
                                    .FontColor(Colors.Black);
                                col.Item().Text($"Name: {auction.Name}").FontColor(Colors.Black);
                                col.Item()
                                    .Text($"Description: {auction.Description}")
                                    .FontColor(Colors.Black);
                                col.Item()
                                    .Text(
                                        $"Starting Price: {auction.StartingPrice.ToString("C", indiaCulture)}"
                                    )
                                    .FontColor(Colors.Black);
                                col.Item()
                                    .Text(
                                        $"Reserve Price: {auction.ReservePrice.ToString("C", indiaCulture)}"
                                    )
                                    .FontColor(Colors.Black)
                                    .Italic();

                                col.Item()
                                    .Text($"Start Time: {localStartTime:yyyy-MM-dd HH:mm}")
                                    .FontColor(Colors.Black);
                                col.Item()
                                    .Text($"End Time: {localEndTime:yyyy-MM-dd HH:mm}")
                                    .FontColor(Colors.Black);
                                col.Item()
                                    .Text($"Status: {auction.Status}")
                                    .FontColor(Colors.Black);
                                col.Item()
                                    .Text($"Seller ID: {auction.SellerId}")
                                    .FontColor(Colors.Black);
                                col.Item()
                                    .Text($"Created At: {localCreatedAt:yyyy-MM-dd HH:mm}")
                                    .FontColor(Colors.Black);
                                col.Item()
                                    .Text($"Updated At: {localUpdatedAt:yyyy-MM-dd HH:mm}")
                                    .FontColor(Colors.Black);

                                col.Item().PaddingVertical(10);

                                col.Item()
                                    .PaddingBottom(5)
                                    .Text("Winning Bid Details")
                                    .FontSize(18)
                                    .Bold()
                                    .FontColor(Colors.Green.Darken2);

                                col.Item()
                                    .Text($"Winning Bid ID: {winningBid.Id}")
                                    .FontColor(Colors.Black);
                                col.Item()
                                    .Text($"Bidder ID: {winningBid.BidderId}")
                                    .FontColor(Colors.Black);
                                col.Item()
                                    .Text(
                                        $"Bid Amount: {winningBid.Amount.ToString("C", indiaCulture)}"
                                    )
                                    .FontColor(Colors.Black);
                                col.Item()
                                    .Text($"Bid Time: {localBidTime:yyyy-MM-dd HH:mm}")
                                    .FontColor(Colors.Black);

                                col.Item().PaddingVertical(10);

                                col.Item()
                                    .Text(
                                        $"Amount Deducted from Virtual Wallet: {winningBid.Amount.ToString("C", indiaCulture)}"
                                    )
                                    .FontColor(Colors.Red.Medium)
                                    .FontSize(16)
                                    .Bold();
                            });
                    });
                })
                .GeneratePdf(ms);
            return ms.ToArray();
        }

        public static string SavePdfToFile(byte[] pdfBytes, string fileName)
        {
            var directory = Path.Combine(Directory.GetCurrentDirectory(), "GeneratedPdfs");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            var filePath = Path.Combine(directory, fileName);
            File.WriteAllBytes(filePath, pdfBytes);
            return filePath;
        }
    }
}
