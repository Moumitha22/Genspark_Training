namespace OnlineAuctionAPI.Models.DTO;

public class DeletedUserDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime DeletedAt { get; set; }
    public string? Reason { get; set; }
}
