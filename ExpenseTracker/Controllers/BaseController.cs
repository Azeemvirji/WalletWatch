using ExpenseTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly UserManager<ApplicationUser> _userManager;

        protected BaseController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        protected Guid GetCurrentUserId()
        {
            var userId = _userManager.GetUserId(User);
            return userId != null ? new Guid(userId) : Guid.Empty;
        }

        protected DateTime GetMonthStartDate(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        protected DateTime GetMonthEndDate(DateTime date)
        {
            return GetMonthStartDate(date).AddMonths(1).AddDays(-1);
        }

        protected async Task<bool> UserIsAdmin()
        {
            var user = await _userManager.GetUserAsync(User);
            return user != null && await _userManager.IsInRoleAsync(user, "Admin");
        }
    }
}
