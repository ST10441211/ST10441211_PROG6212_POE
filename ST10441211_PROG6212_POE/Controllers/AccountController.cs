using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ST10441211_PROG6212_POE.Data;
using ST10441211_PROG6212_POE.Models;
using ST10441211_PROG6212_POE.Services;

namespace ST10441211_PROG6212_POE.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AccountController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var existing = await _db.Users.FirstOrDefaultAsync(u => u.Email == vm.Email);
            if (existing != null)
            {
                ModelState.AddModelError(string.Empty, "Email already registered");
                return View(vm);
            }

            // Create a new user model. We attempt to set PasswordHash if property exists,
            // otherwise set Password (for plaintext storage — not recommended for production).
            var user = new UserModel
            {
                FullName = vm.FullName,
                Email = vm.Email,
                Role = vm.Role
            };

            // If your UserModel has a PasswordHash property, set that. Otherwise set Password.
            var userType = typeof(UserModel);
            var hashProp = userType.GetProperty("PasswordHash", BindingFlags.Public | BindingFlags.Instance);
            var plainProp = userType.GetProperty("Password", BindingFlags.Public | BindingFlags.Instance);

            if (hashProp != null && hashProp.CanWrite)
            {
                // set hashed password
                var hashed = PasswordHasher.Hash(vm.Password);
                hashProp.SetValue(user, hashed);
            }
            else if (plainProp != null && plainProp.CanWrite)
            {
                // fallback: store plaintext password (only for small school projects)
                plainProp.SetValue(user, vm.Password);
            }
            else
            {
                // No writable password property found — fail early
                ModelState.AddModelError(string.Empty, "User model does not have writable Password or PasswordHash property.");
                return View(vm);
            }

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            // auto-login: store identifying info in session
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserRole", user.Role.ToString());
            HttpContext.Session.SetString("UserName", user.FullName);

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            // Find user by email
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == vm.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(vm);
            }

            // Use reflection to determine whether stored password is hashed or plaintext
            var userType = user.GetType();
            var hashProp = userType.GetProperty("PasswordHash", BindingFlags.Public | BindingFlags.Instance);
            var plainProp = userType.GetProperty("Password", BindingFlags.Public | BindingFlags.Instance);

            bool passwordMatches = false;

            if (hashProp != null)
            {
                var storedHash = hashProp.GetValue(user) as string ?? string.Empty;
                if (!string.IsNullOrEmpty(storedHash) && PasswordHasher.Verify(vm.Password, storedHash))
                {
                    passwordMatches = true;
                }
            }
            else if (plainProp != null)
            {
                var storedPlain = plainProp.GetValue(user) as string ?? string.Empty;
                if (storedPlain == vm.Password)
                {
                    passwordMatches = true;
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "User model lacks Password or PasswordHash property.");
                return View(vm);
            }

            if (!passwordMatches)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(vm);
            }

            // Success -> set session and redirect
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserRole", user.Role.ToString());
            HttpContext.Session.SetString("UserName", user.FullName);

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
