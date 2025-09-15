namespace ST10441211_PROG6212_POE.Models
{
    public class ClaimModel
    {
        public int ClaimId { get; set; }
        public int UserId { get; set; } // FK → User
        public string Month { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        public string SupportingDocument { get; set; } = string.Empty; // file path
    }
}
