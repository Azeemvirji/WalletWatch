using System.Collections.Generic;

namespace ExpenseTracker.ViewModels
{
    public class DashboardDataViewModel
    {
        public string TotalIncome { get; set; } = "$0.00";
        public string TotalExpense { get; set; } = "$0.00";
        public string Balance { get; set; } = "$0.00";
        public List<object> ExpenseChartData { get; set; } = new();
        public List<object> IncomeChartData { get; set; } = new();
    }
}
