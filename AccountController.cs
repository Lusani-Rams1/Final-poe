using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CMCS.Services;
using Prog_Poe.Models;
using System.Linq;

namespace Prog_Poe.Controllers
{
    public class AccountController : Controller
    {
        private readonly MockDatabaseService _db;

        public AccountController(MockDatabaseService db)
        {
            _db = db;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email.Equals(email, System.StringComparison.OrdinalIgnoreCase) && u.Password == password);

            if (user != null)
            {
                // Set Session Variables as requested
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("Role", user.Role.ToString());
                HttpContext.Session.SetString("Name", user.Name);
                HttpContext.Session.SetString("LecturerId", user.Id);
                HttpContext.Session.SetString("Role", "Lecturer");


                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid credentials";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}