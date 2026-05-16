using ExpenseTracker.Models;
using ExpenseTracker.Services;
using ExpenseTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class BudgetController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly ITransactionService _transactionService;

        public BudgetController(ApplicationDbContext context, ITransactionService transactionService, UserManager<ApplicationUser> userManager)
            : base(userManager)
        {
            _context = context;
            _transactionService = transactionService;
        }

        public async Task<IActionResult> Index(DateTime? date)
        {
            var selectedDate = date ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var userId = GetCurrentUserId();

            var viewModel = await GetMonthlyBudgetViewModel(userId, selectedDate);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBudget([FromBody] BudgetUpdateModel model)
        {
            var userId = GetCurrentUserId();
            var budget = await _context.Budgets
                .FirstOrDefaultAsync(b => b.CategoryId == model.CategoryId && b.Month == model.Month && b.Year == model.Year && b.UserId == userId);

            if (budget == null)
            {
                budget = new Budget
                {
                    CategoryId = model.CategoryId,
                    Month = model.Month,
                    Year = model.Year,
                    UserId = userId
                };
                _context.Budgets.Add(budget);
            }

            budget.TargetPercentage = model.TargetPercentage;

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] AddCategoryModel model)
        {
            var userId = GetCurrentUserId();
            var existingBudget = await _context.Budgets
                .FirstOrDefaultAsync(b => b.CategoryId == model.CategoryId && b.Month == model.Month && b.Year == model.Year && b.UserId == userId);

            if (existingBudget == null)
            {
                var category = await _context.Categories.FindAsync(model.CategoryId);
                _context.Budgets.Add(new Budget
                {
                    CategoryId = model.CategoryId,
                    Month = model.Month,
                    Year = model.Year,
                    UserId = userId,
                    TargetPercentage = category?.DefaultTargetPercentage ?? 0
                });
                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateExpectedIncome([FromBody] IncomeUpdateModel model)
        {
            var userId = GetCurrentUserId();
            var plan = await _context.MonthlyPlans
                .FirstOrDefaultAsync(p => p.Month == model.Month && p.Year == model.Year && p.UserId == userId);

            if (plan == null)
            {
                plan = new MonthlyPlan
                {
                    Month = model.Month,
                    Year = model.Year,
                    UserId = userId
                };
                _context.MonthlyPlans.Add(plan);
            }

            plan.ExpectedIncome = model.ExpectedIncome;

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UseDefaults(int month, int year)
        {
            var userId = GetCurrentUserId();
            var categories = await _context.Categories
                .Where(c => (c.UserId == userId || c.UserId == Guid.Empty) && c.Type == TransactionType.Expense && c.DefaultTargetPercentage != null)
                .ToListAsync();

            foreach (var category in categories)
            {
                var existingBudget = await _context.Budgets
                    .FirstOrDefaultAsync(b => b.CategoryId == category.CategoryId && b.Month == month && b.Year == year && b.UserId == userId);

                if (existingBudget == null)
                {
                    _context.Budgets.Add(new Budget
                    {
                        CategoryId = category.CategoryId,
                        Month = month,
                        Year = year,
                        UserId = userId,
                        TargetPercentage = category.DefaultTargetPercentage
                    });
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { date = new DateTime(year, month, 1) });
        }

        private async Task<MonthlyBudgetViewModel> GetMonthlyBudgetViewModel(Guid userId, DateTime date)
        {
            var month = date.Month;
            var year = date.Year;

            var startDate = new DateTime(year, month, 1);

            // Get Projected Income from MonthlyPlan
            var plan = await _context.MonthlyPlans
                .FirstOrDefaultAsync(p => p.UserId == userId && p.Month == month && p.Year == year);
            
            float projectedIncome = plan?.ExpectedIncome ?? 0;

            var allExpenseCategories = await _context.Categories
                .Where(c => (c.UserId == userId || c.UserId == Guid.Empty) && c.Type == TransactionType.Expense)
                .ToListAsync();

            var budgets = await _context.Budgets
                .Where(b => b.UserId == userId && b.Month == month && b.Year == year)
                .ToListAsync();

            var categoryBudgets = new List<BudgetViewModel>();
            var availableCategories = new List<Category>();
            float totalAllocated = 0;

            foreach (var category in allExpenseCategories)
            {
                var budget = budgets.FirstOrDefault(b => b.CategoryId == category.CategoryId);
                
                if (budget != null)
                {
                    float? calculatedAmount = null;
                    if (budget.TargetPercentage.HasValue)
                    {
                        calculatedAmount = projectedIncome * (budget.TargetPercentage.Value / 100);
                    }

                    totalAllocated += calculatedAmount ?? 0;

                    categoryBudgets.Add(new BudgetViewModel
                    {
                        CategoryId = category.CategoryId,
                        CategoryTitle = category.Title,
                        CategoryIcon = category.Icon ?? "",
                        TargetPercentage = budget?.TargetPercentage,
                        CalculatedAmount = calculatedAmount
                    });
                }
                else
                {
                    availableCategories.Add(category);
                }
            }

            return new MonthlyBudgetViewModel
            {
                Month = month,
                Year = year,
                SelectedDate = startDate,
                TotalIncome = projectedIncome.ToString("C2"),
                TotalAllocated = totalAllocated.ToString("C2"),
                Unallocated = (projectedIncome - totalAllocated).ToString("C2"),
                CategoryBudgets = categoryBudgets,
                AvailableCategories = availableCategories.OrderBy(c => c.Title).ToList()
            };
        }
    }

    public class BudgetUpdateModel
    {
        public int CategoryId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public float? TargetPercentage { get; set; }
    }

    public class AddCategoryModel
    {
        public int CategoryId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }

    public class IncomeUpdateModel
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public float ExpectedIncome { get; set; }
    }
}
