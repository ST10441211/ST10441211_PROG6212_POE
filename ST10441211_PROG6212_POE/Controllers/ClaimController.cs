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

        // ------------------- INDEX (Landing Page) -------------------
        public IActionResult Index()
        {
            return View();
        }

        // ------------------- CREATE -------------------
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ClaimModel model, IFormFile? SupportingDocument)
        {
            // Remove validation for properties we'll set manually
            ModelState.Remove("LecturerId");
            ModelState.Remove("ClaimDate");
            ModelState.Remove("Status");
            ModelState.Remove("Lecturer");
            ModelState.Remove("SupportingDocumentPath");

            if (!ModelState.IsValid)
            {
                // Log validation errors for debugging
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                }
                return View(model);
            }

            // Handle file upload
            if (SupportingDocument != null && SupportingDocument.Length > 0)
            {
                // Validate file size (5MB max)
                if (SupportingDocument.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("SupportingDocument", "File size cannot exceed 5MB");
                    return View(model);
                }

                // Validate file type
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png", ".xlsx", ".xls" };
                var extension = Path.GetExtension(SupportingDocument.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("SupportingDocument", "Invalid file type. Allowed: PDF, Word, Excel, Images");
                    return View(model);
                }

                try
                {
                    // Create uploads directory if it doesn't exist
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "claims");
                    Directory.CreateDirectory(uploadsFolder);

                    // Generate unique filename
                    var uniqueFileName = $"{Guid.NewGuid()}_{SupportingDocument.FileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        SupportingDocument.CopyTo(stream);
                    }

                    // Store relative path in database
                    model.SupportingDocumentPath = $"/uploads/claims/{uniqueFileName}";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"File upload error: {ex.Message}");
                    ModelState.AddModelError("SupportingDocument", "Error uploading file. Please try again.");
                    return View(model);
                }
            }

            // Set default values if user is not logged in
            if (_session.IsLoggedIn)
            {
                model.LecturerId = _session.GetUserId();
            }
            else
            {
                model.LecturerId = 0; // Default for non-logged in users
            }

            model.ClaimDate = DateTime.UtcNow;
            model.Status = "Pending";

            try
            {
                _context.Claims.Add(model);
                _context.SaveChanges();

                TempData["SuccessMessage"] = $"Claim #{model.ClaimId} created successfully!";
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
                ModelState.AddModelError("", "Error saving claim. Please try again.");
                return View(model);
            }
        }

        // ------------------- READ -------------------
        public IActionResult List()
        {
            var claims = _context.Claims
                .OrderByDescending(c => c.ClaimDate)
                .ToList();

            return View(claims);
        }

        public IActionResult Details(int id)
        {
            var claim = _context.Claims
                .Include(c => c.Lecturer)
                .Include(c => c.ApprovedBy)
                .FirstOrDefault(c => c.ClaimId == id);

            if (claim == null)
                return NotFound();

            return View(claim);
        }

        // ------------------- UPDATE -------------------
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var claim = _context.Claims
                .FirstOrDefault(c => c.ClaimId == id);

            if (claim == null || claim.Status != "Pending")
                return NotFound();

            return View(claim);
        }

        [HttpPost]
        public IActionResult Edit(ClaimModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var claim = _context.Claims
                .FirstOrDefault(c => c.ClaimId == model.ClaimId);

            if (claim == null || claim.Status != "Pending")
                return NotFound();

            claim.ClaimType = model.ClaimType;
            claim.Description = model.Description;

            _context.SaveChanges();

            TempData["SuccessMessage"] = $"Claim #{claim.ClaimId} updated successfully!";
            return RedirectToAction("List");
        }

        // ------------------- DELETE -------------------
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var claim = _context.Claims
                .FirstOrDefault(c => c.ClaimId == id);

            if (claim == null || claim.Status != "Pending")
                return NotFound();

            return View(claim);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var claim = _context.Claims
                .FirstOrDefault(c => c.ClaimId == id);

            if (claim == null || claim.Status != "Pending")
                return NotFound();

            _context.Claims.Remove(claim);
            _context.SaveChanges();

            TempData["SuccessMessage"] = $"Claim #{claim.ClaimId} deleted successfully!";
            return RedirectToAction("List");
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