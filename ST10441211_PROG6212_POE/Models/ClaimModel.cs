namespace ST10441211_PROG6212_POE.Models
{
    public class ClaimModel
    {
        public int ClaimId { get; set; }

        // FK to Lecturer
        public int LecturerId { get; set; }

        // What type of claim (Travel, Hours, Materials, etc.)
        public string ClaimType { get; set; } = string.Empty;

        // Date of claim
        public DateTime ClaimDate { get; set; } = DateTime.UtcNow;

        // Claim amount (money, hours, etc.)
        public decimal Amount { get; set; }

        // Extra details (optional)
        public string Description { get; set; } = string.Empty;

        // Workflow status
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

        // Path or name of supporting document
        public string SupportingDocumentPath { get; set; } = string.Empty;
    }
}
