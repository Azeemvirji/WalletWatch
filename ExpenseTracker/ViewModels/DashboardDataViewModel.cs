using System.Collections.Generic;

namespace ExpenseTracker.ViewModels
{
    public class CategoryProgressViewModel
    {
        public string CategoryTitle { get; set; } = string.Empty;
        public string CategoryIcon { get; set; } = string.Empty;
        public float AllocatedAmount { get; set; }
        public float ActualSpent { get; set; }
        public string FormattedAllocated { get; set; } = "$0.00";
        public string FormattedSpent { get; set; } = "$0.00";
        public float ProgressPercentage { get; set; } // 0 to 100+
    }

    public class DashboardDataViewModel
    {
        public string TotalIncome { get; set; } = "$0.00";
        public string TotalExpense { get; set; } = "$0.00";
        public string Balance { get; set; } = "$0.00";
        public List<object> ExpenseChartData { get; set; } = new();
        public List<object> IncomeChartData { get; set; } = new();
        
        // Budget specific properties
        public bool HasBudget { get; set; } = false;
        public string ProjectedIncome { get; set; } = "$0.00";
        public List<CategoryProgressViewModel> BudgetProgress { get; set; } = new();
    }
}
