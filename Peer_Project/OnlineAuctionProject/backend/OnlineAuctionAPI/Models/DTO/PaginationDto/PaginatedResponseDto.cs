namespace OnlineAuctionAPI.Models.DTO;

public class PaginatedResponseDto<T>
{
    public IEnumerable<T> Data { get; set; } = new List<T>();
    public PaginationMetaData Pagination { get; set; }
}