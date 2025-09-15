namespace ST10441211_PROG6212_POE.Models
{
    public class UserModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // Lecturer, Coordinator, Manager
    }
}
