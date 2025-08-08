namespace OnlineAuctionAPI.Models.DTO;

public class UserSearchDto
{
    public string? SearchTerm { get; set; } = string.Empty;
    public string? SortBy { get; set; }
}