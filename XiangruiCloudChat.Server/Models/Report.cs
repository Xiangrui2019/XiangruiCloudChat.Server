namespace XiangruiCloudChat.Server.Models
{
    public enum ReportStatus
    {
        Pending = 0,
        Resolved = 1
    }
    public class Report
    {
        public int Id { get; set; }

        public string TriggerId { get; set; }
        public ApplicationUser Trigger { get; set; }

        public string TargetId { get; set; }
        public ApplicationUser Target { get; set; }

        public string Reason { get; set; }
        public ReportStatus Status { get; set; } = ReportStatus.Pending;
    }
}
