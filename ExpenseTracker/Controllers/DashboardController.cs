using ExpenseTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class DashboardController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) 
            : base(userManager)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            DateTime date = DateTime.Today;

            DateTime StartDate = GetMonthStartDate(date);
            DateTime EndDate = GetMonthEndDate(date);

            ViewData["Date"] = StartDate;

            await PrepareDateForView(StartDate, EndDate);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(DateTime date)
        {
            DateTime StartDate = GetMonthStartDate(date);
            DateTime EndDate = GetMonthEndDate(date);

            ViewData["Date"] = StartDate;

            await PrepareDateForView(StartDate, EndDate);

            return View();
        }

        public async Task PrepareDateForView(DateTime start, DateTime end)
        {
            List<Transaction> SelectedTransactions = await _context.Transactions
                .Include(x => x.Category)
                .Where(y => y.UserId == GetCurrentUserId() && y.Date >= start && y.Date <= end)
                .ToListAsync();

            //Total Income
            float TotalIncome = SelectedTransactions
                .Where(i => i.Category.Type == "Income")
                .Sum(j => j.Amount);
            ViewBag.TotalIncome = TotalIncome.ToString("C2");

            //Total Expense
            float TotalExpense = SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .Sum(j => j.Amount);
            ViewBag.TotalExpense = TotalExpense.ToString("C2");

            //Balance
            float Balance = TotalIncome - TotalExpense;
            //CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            //culture.NumberFormat.CurrencyNegativePattern = 1;
            //ViewBag.Balance = String.Format(culture, "{0:C2}", Balance);
            ViewBag.Balance = Balance.ToString("C2");

            //Doughnut Chart - Expense By Category
            ViewBag.Expenses = SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .GroupBy(j => j.Category.CategoryId)
                .Select(k => new
                {
                    categoryTitleWithIcon = k.First().Category.TitleWithIcon,
                    amount = Math.Round(k.Sum(j => j.Amount),2),
                    formattedAmount = k.Sum(j => j.Amount).ToString("C2")
                })
                .OrderByDescending(l => l.amount)
                .ToList();

            ViewBag.Income = SelectedTransactions
                .Where(i => i.Category.Type == "Income")
                .GroupBy(j => j.Category.CategoryId)
                .Select(k => new
                {
                    categoryTitleWithIcon = k.First().Category.TitleWithIcon,
                    amount = k.Sum(j => j.Amount),
                    formattedAmount = k.Sum(j => j.Amount).ToString("C2"),
                })
                .OrderByDescending(l => l.amount)
                .ToList();
        }
    }
}
