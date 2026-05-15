using ExpenseTracker.Models;
using ExpenseTracker.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace ExpenseTracker.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext _context;

        public TransactionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardDataViewModel> GetDashboardDataAsync(Guid userId, DateTime startDate, DateTime endDate)
        {
            var selectedTransactions = await _context.Transactions
                .Include(x => x.Category)
                .Where(y => y.UserId == userId && y.Date >= startDate && y.Date <= endDate)
                .ToListAsync();

            // Total Calculations
            float incomeSum = selectedTransactions
                .Where(i => i.Category.Type == CategoryType.Income)
                .Sum(j => j.Amount);

            float expenseSum = selectedTransactions
                .Where(i => i.Category.Type == CategoryType.Expense)
                .Sum(j => j.Amount);

            float balance = incomeSum - expenseSum;

            // Prepare Chart Data
            var expensesByCategory = selectedTransactions
                .Where(i => i.Category.Type == CategoryType.Expense)
                .GroupBy(j => j.Category.CategoryId)
                .Select(k => new
                {
                    categoryTitleWithIcon = k.First().Category.TitleWithIcon,
                    amount = Math.Round(k.Sum(j => j.Amount), 2),
                    formattedAmount = k.Sum(j => j.Amount).ToString("C2")
                })
                .OrderByDescending(l => l.amount)
                .Cast<object>()
                .ToList();

            var incomeByCategory = selectedTransactions
                .Where(i => i.Category.Type == CategoryType.Income)
                .GroupBy(j => j.Category.CategoryId)
                .Select(k => new
                {
                    categoryTitleWithIcon = k.First().Category.TitleWithIcon,
                    amount = k.Sum(j => j.Amount),
                    formattedAmount = k.Sum(j => j.Amount).ToString("C2"),
                })
                .OrderByDescending(l => l.amount)
                .Cast<object>()
                .ToList();

            return new DashboardDataViewModel
            {
                TotalIncome = incomeSum.ToString("C2"),
                TotalExpense = expenseSum.ToString("C2"),
                Balance = balance.ToString("C2"),
                ExpenseChartData = expensesByCategory,
                IncomeChartData = incomeByCategory
            };
        }
    }
}
