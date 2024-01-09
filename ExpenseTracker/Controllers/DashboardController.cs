using ExpenseTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                DateTime date = DateTime.Today;

                DateTime StartDate = GetMonthStartDate(date);
                DateTime EndDate = GetMonthEndDate(date);

                ViewData["Date"] = StartDate;

                await PrepareDateForView(StartDate, EndDate);

                return View();
            }
            catch (Exception ex)
            {
                return RedirectToRoute("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Index(DateTime date)
        {
            try
            {
                DateTime StartDate = GetMonthStartDate(date);
                DateTime EndDate = GetMonthEndDate(date);

                ViewData["Date"] = StartDate;

                await PrepareDateForView(StartDate, EndDate);

                return View();
            }
            catch (Exception ex)
            {
                return RedirectToRoute("Error");
            }
        }

        public async Task PrepareDateForView(DateTime start, DateTime end)
        {
            List<Transaction> SelectedTransactions = await _context.Transactions
                .Include(x => x.Category)
                .Where(y => y.UserId == new Guid(_userManager.GetUserId(this.User)) && y.Date >= start && y.Date <= end)
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

        private DateTime GetMonthStartDate(DateTime date)
        {
            var startDate = new DateTime(date.Year, date.Month, 1);

            return startDate;
        }

        private DateTime GetMonthEndDate(DateTime date)
        {
            var startDate = new DateTime(date.Year, date.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            return endDate;
        }
    }
}
