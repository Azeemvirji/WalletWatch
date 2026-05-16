using System.Collections.Generic;
using ExpenseTracker.Models;

namespace ExpenseTracker.ViewModels
{
    public class BudgetViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryTitle { get; set; } = string.Empty;
        public string CategoryIcon { get; set; } = string.Empty;
        public float? TargetPercentage { get; set; }
        public float? CalculatedAmount { get; set; }
    }

    public class MonthlyBudgetViewModel
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime SelectedDate { get; set; }
        public string TotalIncome { get; set; } = "$0.00";
        public string TotalAllocated { get; set; } = "$0.00";
        public string Unallocated { get; set; } = "$0.00";
        public List<BudgetViewModel> CategoryBudgets { get; set; } = new();
        public List<Category> AvailableCategories { get; set; } = new();
    }
}
