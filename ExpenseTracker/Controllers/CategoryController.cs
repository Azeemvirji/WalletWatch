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
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CategoryController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(this.User);
            var userId = this.GetCurrentUserId();
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
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
            try
            {
                if (id == null || _context.Categories == null)
                {
                    return NotFound();
                }

                var category = await _context.Categories
                    .FirstOrDefaultAsync(m => m.CategoryId == id);
                if (category == null)
                {
                    return NotFound();
                }

                return View(category);
            }
            catch (Exception ex)
            {
                return RedirectToRoute("Error");
            }
        }

        // GET: Category/AddOrEdit
        public IActionResult AddOrEdit(int id = 0)
        {
            try
            {
                if (id == 0)
                {
                    return View(new Category());
                }
                else
                {
                    return View(_context.Categories.Find(id));
                }
            }
            catch (Exception ex)
            {
                return RedirectToRoute("Error");
            }
        }

        // POST: Category/AddOrEdit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("CategoryId,UserId,Title,Icon,Type")] Category category)
        {
            try
            {
                var user = await _userManager.GetUserAsync(this.User);
                var userId = this.GetCurrentUserId();
                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                if (ModelState.IsValid)
                {
                    if (category.CategoryId == 0)
                    {
                        //if (isAdmin)
                        //{
                        //    category.UserId = Guid.Empty;
                        //}
                        //else
                        //{
                        category.UserId = userId;
                        //}

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
            catch (Exception ex)
            {
                return RedirectToRoute("Error");
            }
        }

        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null || _context.Categories == null)
                {
                    return NotFound();
                }

                var category = await _context.Categories
                    .FirstOrDefaultAsync(m => m.CategoryId == id);
                if (category == null)
                {
                    return NotFound();
                }

                return View(category);
            }
            catch (Exception ex)
            {
                return RedirectToRoute("Error");
            }
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(this.User);
                var userId = this.GetCurrentUserId();
                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
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
            catch (Exception ex)
            {
                return RedirectToRoute("Error");
            }
        }

        private bool CategoryExists(int id)
        {
          return (_context.Categories?.Any(e => e.CategoryId == id)).GetValueOrDefault();
        }

        private Guid GetCurrentUserId()
        {
            return new Guid(_userManager.GetUserId(this.User));
        }
    }
}
