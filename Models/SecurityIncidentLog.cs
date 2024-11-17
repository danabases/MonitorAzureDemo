namespace RedisSqlDemo.Models
{
    public class SecurityIncidentLog
    {
        public int IncidentID { get; set; }
        public DateTime DateReported { get; set; }
        public string? ReportedBy { get; set; } // Make nullable
        public int? ThreatID { get; set; } // Nullable for foreign key
        public string? Severity { get; set; } // Make nullable
        public string? Status { get; set; } // Make nullable
        public string? Description { get; set; } // Make nullable
        public ThreatIntelligence? Threat { get; set; } // Make nullable
    }
}