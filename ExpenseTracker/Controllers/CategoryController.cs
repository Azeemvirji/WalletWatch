using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class CategoryController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
            : base(userManager)
        {
            _context = context;
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            var isAdmin = await UserIsAdmin();
            if (isAdmin)
            {
                return _context.Categories != null ?
                              View(await _context.Categories.Where(c => c.UserId == userId || c.UserId == Guid.Empty).OrderByDescending(c => c.UserId).ToListAsync()) :
                              Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
            }
            else
            {
                return _context.Categories != null ?
                              View(await _context.Categories.Where(c => c.UserId == userId).OrderByDescending(c => c.UserId).ToListAsync()) :
                              Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
            }
        }

        // GET: Category/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var userId = GetCurrentUserId();
            var isAdmin = await UserIsAdmin();

            // Allow access if:
            // 1. User is an Admin (can see everything)
            // 2. The category belongs to the user
            // 3. It is a system-created category (Guid.Empty)
            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id && (isAdmin || m.UserId == userId || m.UserId == Guid.Empty));

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Category/AddOrEdit
        public IActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                return View(new Category());
            }
            else
            {
                var userId = GetCurrentUserId();
                var isAdmin = UserIsAdmin().Result;

                // Allow editing if:
                // 1. User is an Admin
                // 2. The category belongs to the user
                // 3. It is a system category AND user is Admin (regular users shouldn't edit system categories)
                var category = _context.Categories
                    .FirstOrDefault(c => c.CategoryId == id && (isAdmin || c.UserId == userId));
                
                if (category == null)
                {
                    return NotFound();
                }
                
                return View(category);
            }
        }

        // POST: Category/AddOrEdit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("CategoryId,UserId,Title,Icon,Type,DefaultTargetAmount,DefaultTargetPercentage")] Category category)
        {
            var userId = GetCurrentUserId();
            var isAdmin = await UserIsAdmin();
            if (ModelState.IsValid)
            {
                if (category.CategoryId == 0)
                {
                    category.UserId = userId;
                    _context.Add(category);
                }
                else
                {
                    if(category.UserId == userId || isAdmin)
                    {
                        _context.Update(category);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var userId = GetCurrentUserId();
            var isAdmin = await UserIsAdmin();

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id && (m.UserId == userId || isAdmin));

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetCurrentUserId();
            var isAdmin = await UserIsAdmin();
            if (_context.Categories == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
            }
            var category = await _context.Categories.FindAsync(id);
            if (category != null && (category.UserId == userId || isAdmin))
            {
                _context.Categories.Remove(category);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
          return (_context.Categories?.Any(e => e.CategoryId == id)).GetValueOrDefault();
        }
    }
}
