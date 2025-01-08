using Mvc_DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Mvc_Models.Models;
using Microsoft.EntityFrameworkCore;

namespace Mvc_Project.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password)
        {
            var user = _db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View();
            }

            if (user.Status == 1) // Check if Status is Inactive (1)
            {
                ModelState.AddModelError("", "Your account is inactive. Please contact support.");
                return View();
            }

            HttpContext.Session.SetInt32("UserID", user.ID);
            HttpContext.Session.SetString("Username", user.Username);

            bool hasExpenses = _db.Expenses.Any(e => e.UserID == user.ID);
            if (!hasExpenses)
            {
                return RedirectToAction("Index", "Categories");
            }

            return RedirectToAction("Index", "Expense");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User user)
        {
            int rowsAffected = 0;

            if (ModelState.IsValid)
            {
                // Set Status to 0 (Active)
                user.Status = 0;

                try
                {
                    _db.Users.Add(user);
                    rowsAffected = _db.SaveChanges();
                    Console.WriteLine($"Rows affected: {rowsAffected}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving user: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while saving the user. Please try again.");
                }

                if (rowsAffected > 0)
                {
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to save user to the database.");
                }
            }

            return View(user);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
