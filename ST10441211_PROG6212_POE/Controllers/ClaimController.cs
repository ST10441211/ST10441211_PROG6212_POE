using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ST10441211_PROG6212_POE.Data;
using ST10441211_PROG6212_POE.Models;
using System.Linq;

namespace ST10441211_PROG6212_POE.Controllers
{
    public class ClaimController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SessionManager _session;

        public ClaimController(ApplicationDbContext context, SessionManager session)
        {
            _context = context;
            _session = session;
        }

        // ------------------- CREATE -------------------
        [HttpGet]
        public IActionResult Create()
        {
            if (!_session.IsLoggedIn)
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        public IActionResult Create(ClaimModel model)
        {
            if (!_session.IsLoggedIn)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            model.LecturerId = _session.GetUserId();
            model.ClaimDate = DateTime.UtcNow;
            model.Status = "Pending";

            _context.Claims.Add(model);
            _context.SaveChanges();

            TempData["SuccessMessage"] = $"Claim #{model.ClaimId} created successfully!";
            return RedirectToAction("MyClaims");
        }

        // ------------------- READ -------------------
        public IActionResult MyClaims()
        {
            if (!_session.IsLoggedIn)
                return RedirectToAction("Login", "Account");

            var claims = _context.Claims
                .Where(c => c.LecturerId == _session.GetUserId())
                .OrderByDescending(c => c.ClaimDate)
                .ToList();

            return View(claims);
        }

        public IActionResult Details(int id)
        {
            if (!_session.IsLoggedIn)
                return RedirectToAction("Login", "Account");

            var claim = _context.Claims
                .Include(c => c.Lecturer)
                .Include(c => c.ApprovedBy)
                .FirstOrDefault(c => c.ClaimId == id);

            if (claim == null)
                return NotFound();

            // Permission check
            if (claim.LecturerId != _session.GetUserId() && _session.GetUserRole() == Role.Lecturer)
                return Forbid();

            return View(claim);
        }

        // ------------------- UPDATE -------------------
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!_session.IsLoggedIn)
                return RedirectToAction("Login", "Account");

            var claim = _context.Claims
                .FirstOrDefault(c => c.ClaimId == id && c.LecturerId == _session.GetUserId());

            if (claim == null || claim.Status != "Pending")
                return Forbid();

            return View(claim);
        }

        [HttpPost]
        public IActionResult Edit(ClaimModel model)
        {
            if (!_session.IsLoggedIn)
                return RedirectToAction("Login", "Account");

            var claim = _context.Claims
                .FirstOrDefault(c => c.ClaimId == model.ClaimId && c.LecturerId == _session.GetUserId());

            if (claim == null || claim.Status != "Pending")
                return Forbid();

            claim.ClaimType = model.ClaimType;
            claim.Description = model.Description;

            _context.SaveChanges();

            TempData["SuccessMessage"] = $"Claim #{claim.ClaimId} updated successfully!";
            return RedirectToAction("MyClaims");
        }

        // ------------------- DELETE -------------------
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (!_session.IsLoggedIn)
                return RedirectToAction("Login", "Account");

            var claim = _context.Claims
                .FirstOrDefault(c => c.ClaimId == id && c.LecturerId == _session.GetUserId());

            if (claim == null || claim.Status != "Pending")
                return Forbid();

            return View(claim);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!_session.IsLoggedIn)
                return RedirectToAction("Login", "Account");

            var claim = _context.Claims
                .FirstOrDefault(c => c.ClaimId == id && c.LecturerId == _session.GetUserId());

            if (claim == null || claim.Status != "Pending")
                return Forbid();

            _context.Claims.Remove(claim);
            _context.SaveChanges();

            TempData["SuccessMessage"] = $"Claim #{claim.ClaimId} deleted successfully!";
            return RedirectToAction("MyClaims");
        }

        // ------------------- REVIEW (Coordinator / Manager) -------------------
        public IActionResult Review()
        {
            if (!_session.IsLoggedIn)
                return RedirectToAction("Login", "Account");

            if (_session.GetUserRole() == Role.Lecturer)
                return Forbid();

            var pendingClaims = _context.Claims
                .Include(c => c.Lecturer)
                .Where(c => c.Status == "Pending")
                .OrderBy(c => c.ClaimDate)
                .ToList();

            return View(pendingClaims);
        }

        [HttpPost]
        public IActionResult ReviewAction(int id, string action)
        {
            if (!_session.IsLoggedIn)
                return RedirectToAction("Login", "Account");

            if (_session.GetUserRole() == Role.Lecturer)
                return Forbid();

            var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == id);

            if (claim == null || claim.Status != "Pending")
                return NotFound();

            if (action == "Approve")
            {
                claim.Status = "Approved";
                claim.ApprovedById = _session.GetUserId();
                claim.DateApproved = DateTime.UtcNow;
            }
            else if (action == "Reject")
            {
                claim.Status = "Rejected";
                claim.ApprovedById = _session.GetUserId();
                claim.DateApproved = DateTime.UtcNow;
            }

            _context.SaveChanges();

            TempData["SuccessMessage"] = $"Claim #{claim.ClaimId} has been {claim.Status.ToLower()}!";
            return RedirectToAction("Review");
        }

        // ------------------- VIEW ALL (Coordinator / Manager) -------------------
        public IActionResult AllClaims()
        {
            if (!_session.IsLoggedIn)
                return RedirectToAction("Login", "Account");

            if (_session.GetUserRole() == Role.Lecturer)
                return Forbid();

            var claims = _context.Claims
                .Include(c => c.Lecturer)
                .OrderByDescending(c => c.ClaimDate)
                .ToList();

            return View(claims);
        }
    }
}
