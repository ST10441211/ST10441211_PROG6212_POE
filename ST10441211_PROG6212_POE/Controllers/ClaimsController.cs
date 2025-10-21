using Microsoft.AspNetCore.Mvc;
using ST10441211_PROG6212_POE.Models;
using System.Collections.Generic;
using System.Linq;

namespace ST10441211_PROG6212_POE.Controllers
{
    public class ClaimsController : Controller
    {
        // Temporary in-memory storage (replace with database context later)
        private static List<ClaimModel> _claims = new List<ClaimModel>
        {
            new ClaimModel
            {
                ClaimId = 1,
                LecturerId = 1,
                ClaimType = "Travel Expense",
                Description = "Conference travel to Johannesburg",
                ClaimDate = System.DateTime.UtcNow.AddDays(-5),
                Status = "Pending"
            },
            new ClaimModel
            {
                ClaimId = 2,
                LecturerId = 1,
                ClaimType = "Accommodation",
                Description = "Hotel booking for seminar",
                ClaimDate = System.DateTime.UtcNow.AddDays(-3),
                Status = "Approved"
            },
            new ClaimModel
            {
                ClaimId = 3,
                LecturerId = 2,
                ClaimType = "Equipment",
                Description = "Laptop for research",
                ClaimDate = System.DateTime.UtcNow.AddDays(-1),
                Status = "Rejected"
            }
        };

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List()
        {
            return View(_claims);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ClaimModel model)
        {
            if (ModelState.IsValid)
            {
                model.ClaimId = _claims.Any() ? _claims.Max(c => c.ClaimId) + 1 : 1;
                model.ClaimDate = System.DateTime.UtcNow;
                model.Status = "Pending";
                _claims.Add(model);
                return RedirectToAction("List");
            }
            return View(model);
        }

        public IActionResult Details(int id)
        {
            var claim = _claims.FirstOrDefault(c => c.ClaimId == id);
            if (claim == null)
            {
                return NotFound();
            }
            return View(claim);
        }

        public IActionResult Edit(int id)
        {
            var claim = _claims.FirstOrDefault(c => c.ClaimId == id);
            if (claim == null)
            {
                return NotFound();
            }
            return View(claim);
        }

        [HttpPost]
        public IActionResult Edit(ClaimModel model)
        {
            if (ModelState.IsValid)
            {
                var claim = _claims.FirstOrDefault(c => c.ClaimId == model.ClaimId);
                if (claim != null)
                {
                    claim.ClaimType = model.ClaimType;
                    claim.Description = model.Description;
                    claim.Status = model.Status;
                }
                return RedirectToAction("List");
            }
            return View(model);
        }

        public IActionResult Support()
        {
            return View();
        }
    }
}