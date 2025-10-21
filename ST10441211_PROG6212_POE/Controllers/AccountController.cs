using ST10441211_PROG6212_POE.Data;
using ST10441211_PROG6212_POE.Models;
using ST10441211_PROG6212_POE.Views;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ST10441211_PROG6212_POE.Controllers
{
    public class AccountController
    {
        private readonly ApplicationDbContext _context;
        private readonly SessionManager _session;
        private readonly ConsoleView _view;

        public AccountController(ApplicationDbContext context, SessionManager session, ConsoleView view)
        {
            _context = context;
            _session = session;
            _view = view;
        }

        public bool Login()
        {
            _view.ShowHeader("Login");

            string email = _view.GetInput("Email");
            string password = _view.GetPassword("Password");

            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null || !VerifyPassword(password, user.PasswordHash))
            {
                _view.ShowError("Invalid email or password.");
                _view.WaitForKey();
                return false;
            }

            _session.Login(user);
            _view.ShowSuccess($"Welcome back, {user.FullName}!");
            System.Threading.Thread.Sleep(1000);
            return true;
        }

        public bool Register()
        {
            _view.ShowHeader("Register New Account");

            string fullName = _view.GetInput("Full Name");
            string email = _view.GetInput("Email");
            string password = _view.GetPassword("Password");

            // Validate email
            if (!IsValidEmail(email))
            {
                _view.ShowError("Invalid email format.");
                _view.WaitForKey();
                return false;
            }

            // Check if email already exists
            if (_context.Users.Any(u => u.Email == email))
            {
                _view.ShowError("Email already registered.");
                _view.WaitForKey();
                return false;
            }

            // Select role
            Console.WriteLine("\nSelect Role:");
            Console.WriteLine("  [1] Lecturer");
            Console.WriteLine("  [2] Programme Coordinator");
            Console.WriteLine("  [3] Academic Manager");
            Console.Write("Choice: ");

            Role role = Role.Lecturer;
            if (int.TryParse(Console.ReadLine(), out int roleChoice))
            {
                role = roleChoice switch
                {
                    2 => Role.ProgrammeCoordinator,
                    3 => Role.AcademicManager,
                    _ => Role.Lecturer
                };
            }

            var user = new UserModel
            {
                FullName = fullName,
                Email = email,
                PasswordHash = HashPassword(password),
                Role = role
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            _view.ShowSuccess("Account created successfully!");
            _view.WaitForKey();
            return true;
        }

        public void Logout()
        {
            _session.Logout();
            _view.ShowSuccess("Logged out successfully.");
            System.Threading.Thread.Sleep(1000);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}