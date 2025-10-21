using ST10441211_PROG6212_POE.Views;
using ST10441211_PROG6212_POE.Models;
using System.Collections.Generic;

namespace ST10441211_PROG6212_POE.Controllers
{
    public class ApplicationController
    {
        private readonly HomeController _homeController;
        private readonly AccountController _accountController;
        private readonly DashboardController _dashboardController;
        private readonly ClaimController _claimController;
        private readonly SessionManager _session;
        private readonly ConsoleView _view;

        public ApplicationController(
            HomeController homeController,
            AccountController accountController,
            DashboardController dashboardController,
            ClaimController claimController,
            SessionManager session,
            ConsoleView view)
        {
            _homeController = homeController;
            _accountController = accountController;
            _dashboardController = dashboardController;
            _claimController = claimController;
            _session = session;
            _view = view;
        }

        public void Run()
        {
            bool running = true;

            while (running)
            {
                if (!_session.IsLoggedIn)
                {
                    running = ShowLoginMenu();
                }
                else
                {
                    ShowMainMenu();
                }
            }

            _view.ShowInfo("Thank you for using the Claims Management System!");
        }

        private bool ShowLoginMenu()
        {
            var options = new List<string>
            {
                "Login",
                "Register",
                "Exit"
            };

            int choice = _view.ShowMenu("Claims Management System", options);

            switch (choice)
            {
                case 1:
                    _accountController.Login();
                    break;
                case 2:
                    _accountController.Register();
                    break;
                case 3:
                    return false;
                default:
                    _view.ShowError("Invalid option. Please try again.");
                    _view.WaitForKey();
                    break;
            }

            return true;
        }

        private void ShowMainMenu()
        {
            var options = new List<string>
            {
                "Home",
                "View Dashboard",
                "--- Claims Management ---",
                "Create New Claim",
                "View My Claims",
                "View Claim Details",
                "Edit Claim",
                "Delete Claim"
            };

            // Add coordinator/manager specific options
            if (_session.GetUserRole() != Role.Lecturer)
            {
                options.Add("--- Review & Administration ---");
                options.Add("Review Pending Claims");
                options.Add("View All Claims");
            }

            options.Add("--- Account ---");
            options.Add("Logout");

            int choice = _view.ShowMenu($"Main Menu - {_session.GetUserName()} ({_session.GetUserRole()})", options);

            // Handle menu choices based on role
            if (_session.GetUserRole() == Role.Lecturer)
            {
                HandleLecturerMenu(choice);
            }
            else
            {
                HandleAdminMenu(choice);
            }
        }

        private void HandleLecturerMenu(int choice)
        {
            switch (choice)
            {
                case 1: // Home
                    _homeController.ShowHomePage();
                    break;
                case 2: // View Dashboard
                    _dashboardController.ShowDashboard();
                    break;
                case 3: // --- Claims Management --- (separator, do nothing)
                    _view.ShowWarning("Please select a valid menu option.");
                    _view.WaitForKey();
                    break;
                case 4: // Create New Claim
                    _claimController.CreateClaim();
                    break;
                case 5: // View My Claims
                    _claimController.ViewMyClaims();
                    break;
                case 6: // View Claim Details
                    _claimController.ViewClaimDetails();
                    break;
                case 7: // Edit Claim
                    _claimController.EditClaim();
                    break;
                case 8: // Delete Claim
                    _claimController.DeleteClaim();
                    break;
                case 9: // --- Account --- (separator, do nothing)
                    _view.ShowWarning("Please select a valid menu option.");
                    _view.WaitForKey();
                    break;
                case 10: // Logout
                    _accountController.Logout();
                    break;
                default:
                    _view.ShowError("Invalid option. Please try again.");
                    _view.WaitForKey();
                    break;
            }
        }

        private void HandleAdminMenu(int choice)
        {
            switch (choice)
            {
                case 1: // Home
                    _homeController.ShowHomePage();
                    break;
                case 2: // View Dashboard
                    _dashboardController.ShowDashboard();
                    break;
                case 3: // --- Claims Management --- (separator)
                    _view.ShowWarning("Please select a valid menu option.");
                    _view.WaitForKey();
                    break;
                case 4: // Create New Claim
                    _claimController.CreateClaim();
                    break;
                case 5: // View My Claims
                    _claimController.ViewMyClaims();
                    break;
                case 6: // View Claim Details
                    _claimController.ViewClaimDetails();
                    break;
                case 7: // Edit Claim
                    _claimController.EditClaim();
                    break;
                case 8: // Delete Claim
                    _claimController.DeleteClaim();
                    break;
                case 9: // --- Review & Administration --- (separator)
                    _view.ShowWarning("Please select a valid menu option.");
                    _view.WaitForKey();
                    break;
                case 10: // Review Pending Claims
                    _claimController.ReviewClaims();
                    break;
                case 11: // View All Claims
                    _claimController.ViewAllClaims();
                    break;
                case 12: // --- Account --- (separator)
                    _view.ShowWarning("Please select a valid menu option.");
                    _view.WaitForKey();
                    break;
                case 13: // Logout
                    _accountController.Logout();
                    break;
                default:
                    _view.ShowError("Invalid option. Please try again.");
                    _view.WaitForKey();
                    break;
            }
        }
    }
}