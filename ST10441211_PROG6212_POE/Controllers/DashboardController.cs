using ST10441211_PROG6212_POE.Data;
using ST10441211_PROG6212_POE.Models;
using ST10441211_PROG6212_POE.Views;
using System.Linq;

namespace ST10441211_PROG6212_POE.Controllers
{
    public class DashboardController
    {
        private readonly ApplicationDbContext _context;
        private readonly SessionManager _session;
        private readonly ConsoleView _view;

        public DashboardController(ApplicationDbContext context, SessionManager session, ConsoleView view)
        {
            _context = context;
            _session = session;
            _view = view;
        }

        public void ShowDashboard()
        {
            if (!_session.IsLoggedIn)
            {
                _view.ShowError("You must be logged in to view the dashboard.");
                _view.WaitForKey();
                return;
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == _session.GetUserId());
            if (user == null)
            {
                _view.ShowError("User not found.");
                _view.WaitForKey();
                return;
            }

            var model = new DashboardViewModel
            {
                CurrentUserName = user.FullName,
                Role = user.Role,
                MyClaims = _context.Claims.Where(c => c.LecturerId == user.Id).ToList(),
                AllClaims = user.Role != Role.Lecturer ? _context.Claims.ToList() : null
            };

            _view.ShowDashboard(model);
        }
    }
}