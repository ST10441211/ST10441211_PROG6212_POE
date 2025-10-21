using ST10441211_PROG6212_POE.Views;
using System;

namespace ST10441211_PROG6212_POE.Controllers
{
    public class HomeController
    {
        private readonly SessionManager _session;
        private readonly ConsoleView _view;

        public HomeController(SessionManager session, ConsoleView view)
        {
            _session = session;
            _view = view;
        }

        public void ShowHomePage()
        {
            _view.ShowHeader("ST10441211 - Insurance Claims Portal");

            Console.WriteLine("Welcome to the Insurance Claims Management System!");
            Console.WriteLine();
            Console.WriteLine("This system allows:");
            Console.WriteLine("  • Lecturers to submit and manage claims");
            Console.WriteLine("  • Programme Coordinators to review claims");
            Console.WriteLine("  • Academic Managers to approve claims");
            Console.WriteLine();

            if (_session.IsLoggedIn)
            {
                _view.ShowSuccess($"You are logged in as: {_session.GetUserName()} ({_session.GetUserRole()})");
            }
            else
            {
                _view.ShowInfo("Please login or register to continue.");
            }

            _view.WaitForKey();
        }
    }
}