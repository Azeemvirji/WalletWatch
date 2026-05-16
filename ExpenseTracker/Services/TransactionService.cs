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
                .Where(i => i.Category != null && i.Category.Type == TransactionType.Income)
                .Sum(j => j.Amount);

            float expenseSum = selectedTransactions
                .Where(i => i.Category != null && i.Category.Type == TransactionType.Expense)
                .Sum(j => j.Amount);

            float balance = incomeSum - expenseSum;

            // Budget Calculations
            var month = startDate.Month;
            var year = startDate.Year;

            var monthlyPlan = await _context.MonthlyPlans
                .FirstOrDefaultAsync(p => p.UserId == userId && p.Month == month && p.Year == year);
            
            var budgets = await _context.Budgets
                .Include(b => b.Category)
                .Where(b => b.UserId == userId && b.Month == month && b.Year == year)
                .ToListAsync();

            float projectedIncome = monthlyPlan?.ExpectedIncome ?? 0;
            bool hasBudget = budgets.Any();

            var budgetProgress = new List<CategoryProgressViewModel>();
            if (hasBudget)
            {
                foreach (var budget in budgets)
                {
                    float allocated = (projectedIncome * (budget.TargetPercentage ?? 0) / 100);
                    float actual = selectedTransactions
                        .Where(t => t.CategoryId == budget.CategoryId)
                        .Sum(t => t.Amount);

                    budgetProgress.Add(new CategoryProgressViewModel
                    {
                        CategoryTitle = budget.Category?.Title ?? "Unknown",
                        CategoryIcon = budget.Category?.Icon ?? "",
                        AllocatedAmount = allocated,
                        ActualSpent = actual,
                        FormattedAllocated = allocated.ToString("C2"),
                        FormattedSpent = actual.ToString("C2"),
                        ProgressPercentage = allocated > 0 ? (actual / allocated) * 100 : (actual > 0 ? 100 : 0)
                    });
                }
            }

            // Prepare Chart Data
            var expensesByCategory = selectedTransactions
                .Where(i => i.Category != null && i.Category.Type == TransactionType.Expense)
                .GroupBy(j => j.Category!.CategoryId)
                .Select(k => new
                {
                    categoryTitleWithIcon = k.First().Category?.TitleWithIcon ?? "Unknown",
                    amount = Math.Round(k.Sum(j => j.Amount), 2),
                    formattedAmount = k.Sum(j => j.Amount).ToString("C2")
                })
                .OrderByDescending(l => l.amount)
                .Cast<object>()
                .ToList();

            var incomeByCategory = selectedTransactions
                .Where(i => i.Category != null && i.Category.Type == TransactionType.Income)
                .GroupBy(j => j.Category!.CategoryId)
                .Select(k => new
                {
                    categoryTitleWithIcon = k.First().Category?.TitleWithIcon ?? "Unknown",
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
                IncomeChartData = incomeByCategory,
                HasBudget = hasBudget,
                ProjectedIncome = projectedIncome.ToString("C2"),
                BudgetProgress = budgetProgress.OrderByDescending(p => p.ProgressPercentage).ToList()
            };
        }
    }
}
