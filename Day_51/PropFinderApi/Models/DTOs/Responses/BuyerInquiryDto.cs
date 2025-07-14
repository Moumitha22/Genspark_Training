namespace PropFinderApi.Models.DTOs.Responses
{ 
    public class BuyerInquiryDto
    {
        public Guid PropertyId { get; set; }
        public string PropertyTitle { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public string ListerName { get; set; } = string.Empty;
        public string ListerEmail { get; set; } = string.Empty;
        public string ListerPhoneNumber { get; set; } = string.Empty;
    }
    
}