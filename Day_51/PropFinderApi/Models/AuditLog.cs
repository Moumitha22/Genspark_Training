namespace PropFinderApi.Models
{
 public class AuditLog
    {
        public Guid Id { get; set; }
        public string TableName { get; set; } = string.Empty;
        public Guid RecordId { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public Guid ChangedByUserId { get; set; }
        public DateTime ChangedAt { get; set; }

        // Navigation
        public User ChangedByUser { get; set; } = null!;
    }   
}