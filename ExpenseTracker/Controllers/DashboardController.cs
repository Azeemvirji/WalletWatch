using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class DashboardController : BaseController
    {
        private readonly ITransactionService _transactionService;

        public DashboardController(ITransactionService transactionService, UserManager<ApplicationUser> userManager) 
            : base(userManager)
        {
            _transactionService = transactionService;
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
            var dashboardData = await _transactionService.GetDashboardDataAsync(GetCurrentUserId(), start, end);

            ViewBag.TotalIncome = dashboardData.TotalIncome;
            ViewBag.TotalExpense = dashboardData.TotalExpense;
            ViewBag.Balance = dashboardData.Balance;
            ViewBag.Expenses = dashboardData.ExpenseChartData;
            ViewBag.Income = dashboardData.IncomeChartData;
        }
    }
}
