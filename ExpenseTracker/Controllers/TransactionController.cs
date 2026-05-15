using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class TransactionController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public TransactionController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
            : base(userManager)
        {
            _context = context;
        }

        // GET: Transaction
        public async Task<IActionResult> Index()
        {
            DateTime date = DateTime.Today;

            DateTime startDate = GetMonthStartDate(date);
            DateTime endDate = GetMonthEndDate(date);

            ViewData["startDate"] = startDate;
            ViewData["endDate"] = endDate;

            var applicationDbContext = _context.Transactions.Where(t => t.UserId == GetCurrentUserId() && t.Date >= startDate && t.Date <= endDate).OrderByDescending(t => t.Date).Include(t => t.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Index(DateTime startDate, DateTime endDate, string button)
        {
            DateTime date = DateTime.Today;

            if (button == DashboardFilter.ThisWeek)
            {
                var start = DayOfWeek.Monday - date.DayOfWeek;

                startDate = date.AddDays(start);
                endDate = startDate.AddDays(6);
            }
            else if (button == DashboardFilter.ThisMonth)
            {
                startDate = GetMonthStartDate(date);
                endDate = GetMonthEndDate(date);
            }

            ViewData["startDate"] = startDate;
            ViewData["endDate"] = endDate;

            var applicationDbContext = _context.Transactions.Where(t => t.UserId == GetCurrentUserId() && t.Date >= startDate && t.Date <= endDate).OrderByDescending(t => t.Date).Include(t => t.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Transaction/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Transactions == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.Category)
                .FirstOrDefaultAsync(m => m.TransactionId == id && m.UserId == GetCurrentUserId());
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transaction/AddOrEdit
        public IActionResult AddOrEdit(int id = 0)
        {
            PopulateCategories();
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId");
            if (id == 0)
                return View(new Transaction());
            else
            {
                var transaction = _context.Transactions.Find(id);
                if (transaction != null && transaction.UserId == GetCurrentUserId())
                {
                    return View(transaction);
                }
                else
                {
                    return NotFound();
                }
            }
        }

        // POST: Transaction/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("TransactionId,UserId,Name,CategoryId,Amount,Note,Date")] Transaction transaction)
        {
            var userId = GetCurrentUserId();
            if (ModelState.IsValid)
            {
                if (transaction.TransactionId == 0)
                {
                    transaction.UserId = userId;
                    _context.Add(transaction);
                }
                else
                {
                    if (transaction.UserId == userId)
                    {
                        _context.Update(transaction);
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateCategories();
            return View(transaction);
        }

        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Transactions == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Transactions'  is null.");
            }
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null && transaction.UserId == GetCurrentUserId())
            {
                _context.Transactions.Remove(transaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(int id)
        {
          return (_context.Transactions?.Any(e => e.TransactionId == id)).GetValueOrDefault();
        }

        [NonAction]
        public void PopulateCategories()
        {
            var userId = GetCurrentUserId();
            var isAdmin = UserIsAdmin().Result;

            if (isAdmin)
            {
                ViewBag.Categories = _context.Categories.Where(c => c.UserId == userId || c.UserId == Guid.Empty).ToList();
            }
            else
            {
                ViewBag.Categories = _context.Categories.Where(c => c.UserId == userId).ToList();
            }

            //Category DefaultCategory = new Category() { CategoryId = 0, Title = "Choose a Category" };
            //CategoryCollection.Insert(0, DefaultCategory);
            //ViewBag.Categories = CategoryCollection;
        }
    }
}
