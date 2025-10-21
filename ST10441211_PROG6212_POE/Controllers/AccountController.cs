using Microsoft.AspNetCore.Mvc;
using ST10441211_PROG6212_POE.Data;
using ST10441211_PROG6212_POE.Models;
using System.Linq;

namespace ST10441211_PROG6212_POE.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SessionManager _session;

        public AccountController(ApplicationDbContext context, SessionManager session)
        {
            _context = context;
            _session = session;
        }

        // ------------------- LOGIN -------------------
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.ErrorMessage = "Please enter both email and password.";
                return View();
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                ViewBag.ErrorMessage = "No user found with that email.";
                return View();
            }

            // For now, we’ll skip password hashing logic
            if (user.PasswordHash != password)
            {
                ViewBag.ErrorMessage = "Incorrect password.";
                return View();
            }

            // Start session using the corrected SessionManager
            _session.Login(user);

            TempData["SuccessMessage"] = "Login successful!";
            return RedirectToAction("Index", "Dashboard");
        }

        // ------------------- REGISTER -------------------
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Check if email exists
            if (_context.Users.Any(u => u.Email == model.Email))
            {
                ViewBag.ErrorMessage = "Email already registered.";
                return View(model);
            }

            var user = new UserModel
            {
                FullName = model.FullName,
                Email = model.Email,
                PasswordHash = model.Password, // ⚠ In real apps, hash this
                Role = model.Role
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Registration successful! Please log in.";
            return RedirectToAction("Login");
        }

        // ------------------- LOGOUT -------------------
        public IActionResult Logout()
        {
            // End session using the corrected SessionManager
            _session.Logout();

            TempData["SuccessMessage"] = "You have been logged out.";
            return RedirectToAction("Login");
        }
    }
}
