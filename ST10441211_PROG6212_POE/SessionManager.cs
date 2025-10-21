using Microsoft.AspNetCore.Http;
using ST10441211_PROG6212_POE.Models;

namespace ST10441211_PROG6212_POE
{
    public class SessionManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // ------------------- LOGIN -------------------
        public void Login(UserModel user)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return;

            session.SetInt32("UserId", user.Id);
            session.SetString("UserEmail", user.Email);
            session.SetString("UserRole", user.Role.ToString());
            session.SetString("UserFullName", user.FullName);
        }

        // ------------------- LOGOUT -------------------
        public void Logout()
        {
            _httpContextAccessor.HttpContext?.Session.Clear();
        }

        // ------------------- PROPERTIES -------------------
        public bool IsLoggedIn => _httpContextAccessor.HttpContext?.Session.GetInt32("UserId") != null;

        public int GetUserId() => _httpContextAccessor.HttpContext?.Session.GetInt32("UserId") ?? 0;

        public string GetUserEmail() => _httpContextAccessor.HttpContext?.Session.GetString("UserEmail") ?? string.Empty;

        public string GetUserName() => _httpContextAccessor.HttpContext?.Session.GetString("UserFullName") ?? string.Empty;

        public Role GetUserRole()
        {
            var roleString = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");
            return Enum.TryParse<Role>(roleString, out var role) ? role : Role.Lecturer;
        }
    }
}
