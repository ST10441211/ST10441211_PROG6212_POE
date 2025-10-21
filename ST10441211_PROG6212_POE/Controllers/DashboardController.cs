using Microsoft.AspNetCore.Mvc;
using ST10441211_PROG6212_POE.Data;
using ST10441211_PROG6212_POE.Models;
using System.Linq;

namespace ST10441211_PROG6212_POE.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SessionManager _session;

        public DashboardController(ApplicationDbContext context, SessionManager session)
        {
            _context = context;
            _session = session;
        }

        public IActionResult Index()
        {
            UserModel? user;

            if (_session.IsLoggedIn)
            {
                // Get the logged-in user from session
                var userId = _session.GetUserId();
                user = _context.Users.FirstOrDefault(u => u.Id == userId);
            }
            else
            {
                // No user logged in yet, use a default/test user
                user = _context.Users.FirstOrDefault(); // Pick the first user in DB
                if (user == null)
                {
                    // If no users exist, create a temporary in-memory user
                    user = new UserModel
                    {
                        Id = 0,
                        FullName = "Test User",
                        Email = "test@example.com",
                        Role = Role.Lecturer
                    };
                }
            }

            // Build dashboard model
            var model = new DashboardViewModel
            {
                CurrentUserName = user.FullName,
                Role = user.Role,
                MyClaims = _context.Claims
                    .Where(c => c.LecturerId == user.Id)
                    .ToList(),
                AllClaims = user.Role != Role.Lecturer
                    ? _context.Claims.ToList()
                    : null
            };

            return View(model); // MVC automatically uses Views/Dashboard/Index.cshtml
        }
    }
}
