namespace PropFinderApi.Models.DTOs.Responses
{ 
    public class ListerInquiryDto
    {
        public Guid PropertyId { get; set; }
        public string PropertyTitle { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;

        public string BuyerEmail { get; set; } = string.Empty;
        public string BuyerPhoneNumber { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }

    
}