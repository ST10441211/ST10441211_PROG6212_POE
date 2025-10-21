using ST10441211_PROG6212_POE.Models;

namespace ST10441211_PROG6212_POE
{
    public class SessionManager
    {
        private UserModel? _currentUser;

        public UserModel? CurrentUser
        {
            get => _currentUser;
            set => _currentUser = value;
        }

        public bool IsLoggedIn => _currentUser != null;

        public void Login(UserModel user)
        {
            _currentUser = user;
        }

        public void Logout()
        {
            _currentUser = null;
        }

        public string GetUserEmail()
        {
            return _currentUser?.Email ?? string.Empty;
        }

        public int GetUserId()
        {
            return _currentUser?.Id ?? 0;
        }

        public Role GetUserRole()
        {
            return _currentUser?.Role ?? Role.Lecturer;
        }

        public string GetUserName()
        {
            return _currentUser?.FullName ?? string.Empty;
        }
    }
}