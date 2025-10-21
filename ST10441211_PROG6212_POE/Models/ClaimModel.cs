using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ST10441211_PROG6212_POE.Models
{
    public class ClaimModel
    {
        [Key]
        public int ClaimId { get; set; } // Changed from Id to ClaimId

        [Required]
        public int LecturerId { get; set; }

        [ForeignKey("LecturerId")]
        public UserModel Lecturer { get; set; } = null!;

        [Required]
        public string ClaimType { get; set; } = string.Empty; // Changed from Title to ClaimType

        public string Description { get; set; } = string.Empty;

        public DateTime ClaimDate { get; set; } = DateTime.UtcNow; // Changed from DateSubmitted to ClaimDate

        public string Status { get; set; } = "Pending"; // Changed from Approved (bool) to Status (string)

        public int? ApprovedById { get; set; }

        [ForeignKey("ApprovedById")]
        public UserModel? ApprovedBy { get; set; }

        public DateTime? DateApproved { get; set; }
    }
}