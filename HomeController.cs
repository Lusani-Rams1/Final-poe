using CMCS.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Prog_Poe.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CMCS.Controllers
{
    public class HomeController : Controller
    {
        private readonly MockDatabaseService _db;
        private readonly IWebHostEnvironment _env;

        public HomeController(MockDatabaseService db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // -------------------------
        // DASHBOARD ROUTING
        // -------------------------
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role == "Lecturer")
            {
                var lecturerId = HttpContext.Session.GetString("LecturerId");
                var claims = _db.Claims.Where(c => c.LecturerId == lecturerId).ToList();
                return View("LecturerDashboard", claims);
            }

            if (role == "Coordinator")
            {
                var claims = _db.Claims.ToList();
                return View("CoordinatorDashboard", claims);
            }

            return RedirectToAction("Login", "Account");
        }

        // -------------------------
        // SUBMIT CLAIM
        // -------------------------
        [HttpGet]
        public IActionResult SubmitClaim()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitClaim(double hoursWorked, double hourlyRate, string description, IFormFile file)
        {
            var lecturerId = HttpContext.Session.GetString("LecturerId");
            var lecturerName = HttpContext.Session.GetString("Name");

            if (lecturerId == null)
            {
                ModelState.AddModelError("", "Session expired. Please log in again.");
                return View();
            }

            string savedFileName = null;

            if (file != null && file.Length > 0)
            {
                // take original filename, sanitize, and prefix with a GUID to avoid collisions
                var originalFileName = Path.GetFileName(file.FileName) ?? "upload";
                var ext = Path.GetExtension(originalFileName);
                var nameOnly = Path.GetFileNameWithoutExtension(originalFileName);

                // sanitize invalid filename chars
                var invalids = Path.GetInvalidFileNameChars();
                foreach (var c in invalids)
                {
                    nameOnly = nameOnly.Replace(c, '_');
                }

                // limit length to avoid filesystem issues
                if (nameOnly.Length > 100) nameOnly = nameOnly.Substring(0, 100);

                savedFileName = $"{Guid.NewGuid():N}_{nameOnly}{ext}";

                // resolve physical uploads directory under webroot and ensure it exists
                var uploadsDir = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsDir);

                var filePath = Path.Combine(uploadsDir, savedFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }

            var newClaim = new Claim
            {
                Id = "CLM-" + new Random().Next(1000, 9999),
                LecturerId = lecturerId,
                LecturerName = lecturerName,
                SubmissionDate = DateTime.Now,
                HoursWorked = hoursWorked,
                HourlyRate = hourlyRate,
                Description = description,
                FileName = savedFileName,
                Status = ClaimStatus.Pending
            };

            _db.Claims.Add(newClaim);

            TempData["Success"] = "Claim submitted successfully!";
            return RedirectToAction("Index");
        }

        // -------------------------
        // APPROVE CLAIM
        // -------------------------
        public IActionResult Approve(string id)
        {
            var claim = _db.Claims.FirstOrDefault(c => c.Id == id);
            if (claim == null) return NotFound();

            claim.Status = ClaimStatus.Approved;
            claim.ReviewerName = HttpContext.Session.GetString("Name");
            claim.RejectionReason = null;

            TempData["Success"] = "Claim approved.";
            return RedirectToAction("Index");
        }

        // -------------------------
        // REJECT CLAIM (GET)
        // -------------------------
        [HttpGet]
        public IActionResult Reject(string id)
        {
            var claim = _db.Claims.FirstOrDefault(c => c.Id == id);
            return View("RejectClaim", claim);
        }

        // -------------------------
        // REJECT CLAIM (POST)
        // -------------------------
        [HttpPost]
        public IActionResult Reject(string id, string rejectionReason)
        {
            var claim = _db.Claims.FirstOrDefault(c => c.Id == id);

            if (string.IsNullOrWhiteSpace(rejectionReason))
            {
                ModelState.AddModelError("", "Rejection reason is required.");
                return View("RejectClaim", claim);
            }

            claim.Status = ClaimStatus.Rejected;
            claim.ReviewerName = HttpContext.Session.GetString("Name");
            claim.RejectionReason = rejectionReason;

            TempData["Success"] = "Claim rejected.";
            return RedirectToAction("Index");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }


    }
}
