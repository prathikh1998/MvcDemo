using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mvc_DataAccess.Data;
using Mvc_Models.Models;

namespace Mvc_Project.Controllers
{
    public class BudgetController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BudgetController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            var budgets = _context.Budgets.Where(b => b.UserID == userId).ToList();
            return View(budgets);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Budget budget)
        {
            if (ModelState.IsValid)
            {
                budget.UserID = HttpContext.Session.GetInt32("UserID").GetValueOrDefault();

                _context.Budgets.Add(budget);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(budget);
        }

        public IActionResult Edit(int id)
        {
            var budget = _context.Budgets.Find(id);
            if (budget == null)
            {
                return NotFound();
            }

            return View(budget);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Budget budget)
        {
            if (ModelState.IsValid)
            {
                _context.Budgets.Update(budget);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(budget);
        }

        public IActionResult Delete(int id)
        {
            var budget = _context.Budgets.Find(id);
            if (budget == null)
            {
                return NotFound();
            }

            _context.Budgets.Remove(budget);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
