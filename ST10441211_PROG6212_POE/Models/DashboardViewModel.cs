using System.Collections.Generic;

namespace ST10441211_PROG6212_POE.Models
{
    public class DashboardViewModel
    {
        public string CurrentUserName { get; set; } = string.Empty;

        public Role Role { get; set; }

        public List<ClaimModel> MyClaims { get; set; } = new List<ClaimModel>();

        public List<ClaimModel>? AllClaims { get; set; }
    }
}