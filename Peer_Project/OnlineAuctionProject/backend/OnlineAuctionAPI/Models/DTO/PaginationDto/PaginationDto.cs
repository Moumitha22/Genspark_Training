namespace OnlineAuctionAPI.Models.DTO;

public class PaginationDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Name { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; } = "asc";
    public string? Status { get; set; }
    public decimal? StartingPriceMin { get; set; }
    public decimal? StartingPriceMax { get; set; }
    public decimal? ReservePriceMin { get; set; }
    public decimal? ReservePriceMax { get; set; }
    public bool? HasFileAttachments { get; set; }
    public string? FileName { get; set; }
    public string? SellerId { get; set; }
}