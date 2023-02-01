using ExpenseTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            //var applicationDbContext = _context.Transactions.Include(t => t.Category);
            var income = this.SortTransactionsByCategories("Income");
            var expense = this.SortTransactionsByCategories("Expense");
            var output = new Tuple<List<CategoriesWithAmount>, List<CategoriesWithAmount>>(income, expense);
            return View(output);
        }

        private List<CategoriesWithAmount> SortTransactionsByCategories(string type)
        {
            var categories = new List<CategoriesWithAmount>();
            float totalAmount = 0;

            var applicationDbContext = _context.Transactions.Where(t => t.UserId == new Guid(_userManager.GetUserId(this.User)) && t.Category.Type == type).Include(t => t.Category);

            foreach(var x in applicationDbContext)
            {
                totalAmount += x.Amount;
                if(!categories.Exists(c => c.Category.CategoryId == x.CategoryId))
                {
                    categories.Add(new CategoriesWithAmount(x.Category, x.Amount));
                }
                else
                {
                    categories.Find(c => c.Category.CategoryId == x.CategoryId).AddAmount(x.Amount);
                }
            }

            var total = new Category();
            total.Title = "Total";
            total.Icon = "=";
            if(totalAmount > 0)
                categories.Add(new CategoriesWithAmount(total, totalAmount));

            return categories;
        }
    }

    public class CategoriesWithAmount
    {
        public Category Category { get; set; }
        public float Amount { get; set; }

        public CategoriesWithAmount() { }

        public CategoriesWithAmount(Category category, float amount) {
            Category = category;
            Amount = amount;
        }

        public void AddAmount(float amount)
        {
            Amount += amount;
        }
    }
}
