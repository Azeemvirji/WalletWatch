using ExpenseTracker.ViewModels;

namespace ExpenseTracker.Services
{
    public interface ITransactionService
    {
        Task<DashboardDataViewModel> GetDashboardDataAsync(Guid userId, DateTime startDate, DateTime endDate);
    }
}
