using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mvc_DataAccess.Data;
using Mvc_Models.Models;

namespace Mvc_Project.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExpenseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Expense
        public IActionResult Index()
        {
            int userId = HttpContext.Session.GetInt32("UserID") ?? 0;

            if (userId == 0)
            {
                return RedirectToAction("Login", "User");
            }

            var expenses = _context.Expenses
        .Where(e => e.UserID == userId)
        .ToList();

            return View(expenses);
        }

        // GET: Expense/Create
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        // POST: Expense/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Expense expense)
        {
            if (ModelState.IsValid)
            {
                expense.UserID = HttpContext.Session.GetInt32("UserID") ?? 0;

                _context.Expenses.Add(expense);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(expense);
        }

        // GET: Expense/Edit/{id}
        public IActionResult Edit(int id)
        {
            var expense = _context.Expenses.Find(id);
            if (expense == null)
            {
                return NotFound();
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(expense);
        }

        // POST: Expense/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Expense expense)
        {
            if (ModelState.IsValid)
            {
                _context.Expenses.Update(expense);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(expense);
        }

        // GET: Expense/Delete/{id}
        public IActionResult Delete(int id)
        {
            var expense = _context.Expenses.Find(id);
            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        // POST: Expense/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var expense = _context.Expenses.Find(id);
            if (expense != null)
            {
                _context.Expenses.Remove(expense);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
