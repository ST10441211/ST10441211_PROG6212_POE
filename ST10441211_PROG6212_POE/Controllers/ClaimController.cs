using ST10441211_PROG6212_POE.Data;
using ST10441211_PROG6212_POE.Models;
using ST10441211_PROG6212_POE.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ST10441211_PROG6212_POE.Controllers
{
    public class ClaimController
    {
        private readonly ApplicationDbContext _context;
        private readonly SessionManager _session;
        private readonly ConsoleView _view;

        public ClaimController(ApplicationDbContext context, SessionManager session, ConsoleView view)
        {
            _context = context;
            _session = session;
            _view = view;
        }

        // CREATE - Submit a new claim
        public void CreateClaim()
        {
            if (!_session.IsLoggedIn)
            {
                _view.ShowError("You must be logged in to submit a claim.");
                _view.WaitForKey();
                return;
            }

            _view.ShowHeader("Create New Claim");

            string claimType = _view.GetInput("Claim Type (e.g., Overtime, Travel, Materials)");

            if (string.IsNullOrWhiteSpace(claimType))
            {
                _view.ShowError("Claim type cannot be empty.");
                _view.WaitForKey();
                return;
            }

            string description = _view.GetInput("Description");

            var claim = new ClaimModel
            {
                LecturerId = _session.GetUserId(),
                ClaimType = claimType,
                Description = description,
                ClaimDate = DateTime.UtcNow,
                Status = "Pending"
            };

            _context.Claims.Add(claim);
            _context.SaveChanges();

            _view.ShowSuccess($"Claim #{claim.ClaimId} created successfully!");
            _view.WaitForKey();
        }

        // READ - View all claims for current user
        public void ViewMyClaims()
        {
            if (!_session.IsLoggedIn)
            {
                _view.ShowError("You must be logged in.");
                _view.WaitForKey();
                return;
            }

            _view.ShowHeader("My Claims");

            var claims = _context.Claims
                .Where(c => c.LecturerId == _session.GetUserId())
                .OrderByDescending(c => c.ClaimDate)
                .ToList();

            if (claims.Count == 0)
            {
                _view.ShowInfo("You have no claims yet.");
            }
            else
            {
                _view.ShowClaimsList(claims);
            }

            _view.WaitForKey();
        }

        // READ - View specific claim details
        public void ViewClaimDetails()
        {
            if (!_session.IsLoggedIn)
            {
                _view.ShowError("You must be logged in.");
                _view.WaitForKey();
                return;
            }

            _view.ShowHeader("View Claim Details");

            Console.Write("Enter Claim ID: ");
            if (!int.TryParse(Console.ReadLine(), out int claimId))
            {
                _view.ShowError("Invalid Claim ID.");
                _view.WaitForKey();
                return;
            }

            var claim = _context.Claims
                .Include(c => c.Lecturer)
                .Include(c => c.ApprovedBy)
                .FirstOrDefault(c => c.ClaimId == claimId);

            if (claim == null)
            {
                _view.ShowError($"Claim #{claimId} not found.");
                _view.WaitForKey();
                return;
            }

            // Check permissions - users can only view their own claims unless they're coordinators/managers
            if (claim.LecturerId != _session.GetUserId() && _session.GetUserRole() == Role.Lecturer)
            {
                _view.ShowError("You don't have permission to view this claim.");
                _view.WaitForKey();
                return;
            }

            Console.WriteLine("\n╔═══════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"  Claim ID: {claim.ClaimId}");
            Console.WriteLine($"  Lecturer: {claim.Lecturer.FullName} ({claim.Lecturer.Email})");
            Console.WriteLine($"  Claim Type: {claim.ClaimType}");
            Console.WriteLine($"  Description: {claim.Description}");
            Console.WriteLine($"  Claim Date: {claim.ClaimDate:yyyy-MM-dd HH:mm}");
            Console.WriteLine($"  Status: {claim.Status}");

            if (claim.ApprovedBy != null)
            {
                Console.WriteLine($"  Reviewed By: {claim.ApprovedBy.FullName}");
                Console.WriteLine($"  Review Date: {claim.DateApproved:yyyy-MM-dd HH:mm}");
            }

            Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");

            _view.WaitForKey();
        }

        // UPDATE - Edit an existing claim
        public void EditClaim()
        {
            if (!_session.IsLoggedIn)
            {
                _view.ShowError("You must be logged in.");
                _view.WaitForKey();
                return;
            }

            _view.ShowHeader("Edit Claim");

            Console.Write("Enter Claim ID to edit: ");
            if (!int.TryParse(Console.ReadLine(), out int claimId))
            {
                _view.ShowError("Invalid Claim ID.");
                _view.WaitForKey();
                return;
            }

            var claim = _context.Claims
                .FirstOrDefault(c => c.ClaimId == claimId && c.LecturerId == _session.GetUserId());

            if (claim == null)
            {
                _view.ShowError($"Claim #{claimId} not found or you don't have permission to edit it.");
                _view.WaitForKey();
                return;
            }

            if (claim.Status != "Pending")
            {
                _view.ShowError($"Cannot edit claim with status '{claim.Status}'. Only pending claims can be edited.");
                _view.WaitForKey();
                return;
            }

            Console.WriteLine($"\nCurrent Claim Type: {claim.ClaimType}");
            string newClaimType = _view.GetInput("New Claim Type (press Enter to keep current)");
            if (!string.IsNullOrWhiteSpace(newClaimType))
            {
                claim.ClaimType = newClaimType;
            }

            Console.WriteLine($"\nCurrent Description: {claim.Description}");
            string newDescription = _view.GetInput("New Description (press Enter to keep current)");
            if (!string.IsNullOrWhiteSpace(newDescription))
            {
                claim.Description = newDescription;
            }

            _context.SaveChanges();

            _view.ShowSuccess($"Claim #{claimId} updated successfully!");
            _view.WaitForKey();
        }

        // DELETE - Remove a claim
        public void DeleteClaim()
        {
            if (!_session.IsLoggedIn)
            {
                _view.ShowError("You must be logged in.");
                _view.WaitForKey();
                return;
            }

            _view.ShowHeader("Delete Claim");

            Console.Write("Enter Claim ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int claimId))
            {
                _view.ShowError("Invalid Claim ID.");
                _view.WaitForKey();
                return;
            }

            var claim = _context.Claims
                .FirstOrDefault(c => c.ClaimId == claimId && c.LecturerId == _session.GetUserId());

            if (claim == null)
            {
                _view.ShowError($"Claim #{claimId} not found or you don't have permission to delete it.");
                _view.WaitForKey();
                return;
            }

            if (claim.Status != "Pending")
            {
                _view.ShowError($"Cannot delete claim with status '{claim.Status}'. Only pending claims can be deleted.");
                _view.WaitForKey();
                return;
            }

            Console.WriteLine($"\nClaim Type: {claim.ClaimType}");
            Console.WriteLine($"Description: {claim.Description}");
            Console.Write("\nAre you sure you want to delete this claim? (yes/no): ");

            string confirmation = Console.ReadLine()?.ToLower() ?? "";

            if (confirmation == "yes" || confirmation == "y")
            {
                _context.Claims.Remove(claim);
                _context.SaveChanges();

                _view.ShowSuccess($"Claim #{claimId} deleted successfully!");
            }
            else
            {
                _view.ShowInfo("Delete operation cancelled.");
            }

            _view.WaitForKey();
        }

        // REVIEW - For Coordinators and Managers
        public void ReviewClaims()
        {
            if (!_session.IsLoggedIn)
            {
                _view.ShowError("You must be logged in.");
                _view.WaitForKey();
                return;
            }

            if (_session.GetUserRole() == Role.Lecturer)
            {
                _view.ShowError("Only Coordinators and Managers can review claims.");
                _view.WaitForKey();
                return;
            }

            _view.ShowHeader("Review Pending Claims");

            var pendingClaims = _context.Claims
                .Include(c => c.Lecturer)
                .Where(c => c.Status == "Pending")
                .OrderBy(c => c.ClaimDate)
                .ToList();

            if (pendingClaims.Count == 0)
            {
                _view.ShowInfo("No pending claims to review.");
                _view.WaitForKey();
                return;
            }

            Console.WriteLine($"Found {pendingClaims.Count} pending claim(s)\n");

            foreach (var claim in pendingClaims)
            {
                Console.WriteLine("═══════════════════════════════════════════════════════════════");
                Console.WriteLine($"Claim ID: {claim.ClaimId}");
                Console.WriteLine($"Lecturer: {claim.Lecturer.FullName} ({claim.Lecturer.Email})");
                Console.WriteLine($"Type: {claim.ClaimType}");
                Console.WriteLine($"Description: {claim.Description}");
                Console.WriteLine($"Date: {claim.ClaimDate:yyyy-MM-dd HH:mm}");
                Console.WriteLine($"Status: {claim.Status}");
                Console.WriteLine("═══════════════════════════════════════════════════════════════");

                Console.WriteLine("\n[1] Approve  [2] Reject  [3] Skip  [4] Exit Review");
                Console.Write("Action: ");

                if (int.TryParse(Console.ReadLine(), out int action))
                {
                    if (action == 1)
                    {
                        claim.Status = "Approved";
                        claim.ApprovedById = _session.GetUserId();
                        claim.DateApproved = DateTime.UtcNow;
                        _context.SaveChanges();
                        _view.ShowSuccess($"Claim #{claim.ClaimId} approved!");
                    }
                    else if (action == 2)
                    {
                        claim.Status = "Rejected";
                        claim.ApprovedById = _session.GetUserId();
                        claim.DateApproved = DateTime.UtcNow;
                        _context.SaveChanges();
                        _view.ShowWarning($"Claim #{claim.ClaimId} rejected.");
                    }
                    else if (action == 4)
                    {
                        break;
                    }
                }

                Console.WriteLine();
            }

            _view.ShowSuccess("Review session completed.");
            _view.WaitForKey();
        }

        // LIST - View all claims (for Coordinators/Managers)
        public void ViewAllClaims()
        {
            if (!_session.IsLoggedIn)
            {
                _view.ShowError("You must be logged in.");
                _view.WaitForKey();
                return;
            }

            if (_session.GetUserRole() == Role.Lecturer)
            {
                _view.ShowError("Only Coordinators and Managers can view all claims.");
                _view.WaitForKey();
                return;
            }

            _view.ShowHeader("All Claims");

            var claims = _context.Claims
                .Include(c => c.Lecturer)
                .OrderByDescending(c => c.ClaimDate)
                .ToList();

            if (claims.Count == 0)
            {
                _view.ShowInfo("No claims in the system.");
            }
            else
            {
                Console.WriteLine($"Total Claims: {claims.Count}\n");

                foreach (var claim in claims)
                {
                    Console.WriteLine($"ID: {claim.ClaimId} | Lecturer: {claim.Lecturer.FullName} | Type: {claim.ClaimType}");
                    Console.WriteLine($"Date: {claim.ClaimDate:yyyy-MM-dd} | Status: {claim.Status}");
                    Console.WriteLine(new string('-', 60));
                }
            }

            _view.WaitForKey();
        }
    }
}