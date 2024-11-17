namespace RedisSqlDemo.Models
{
    public class ThreatIntelligence
    {
        public int ThreatID { get; set; }
        public string? ThreatName { get; set; } // Make nullable
        public string? ThreatType { get; set; } // Make nullable
        public string? Severity { get; set; } // Make nullable
        public string? Indicators { get; set; } // Make nullable
        public DateTime? LastSeen { get; set; } // Make nullable
    }
}