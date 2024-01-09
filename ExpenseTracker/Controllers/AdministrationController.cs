using ExpenseTracker.ViewModels;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager, 
            UserManager<ApplicationUser> userManager) 
        { 
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var users = _userManager.Users.ToList();

                var model = new UsersViewModel();

                var usersList = new List<Users>();

                foreach (var user in users)
                {
                    var admin = _userManager.IsInRoleAsync(user, "Admin");

                    usersList.Add(new Users
                    {
                        Id = user.Id,
                        Email = user.Email,
                        LastLoggedIn = user.LastLoggedIn.ToLongDateString(),
                        Role = admin.Result ? "Admin" : "User" // If not admin then give user role
                    });
                }

                model.users = usersList;

                return View(model);
            }
            catch(Exception ex)
            {
                return RedirectToRoute("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string Id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(Id);

                if (user == null)
                {
                    return NotFound();
                }
                else
                {
                    var result = _userManager.DeleteAsync(user);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error");
            }
        }

        [HttpPost, ActionName("MakeAdmin")]
        public async Task<IActionResult> MakeAdmin(string Id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(Id);

                if (user == null)
                {
                    return NotFound();
                }

                var result = await _userManager.AddToRoleAsync(user, "Admin");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToRoute("Error");
            }
        }
        
        [HttpPost, ActionName("RemoveAdmin")]
        public async Task<IActionResult> RemoveAdmin(string Id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(Id);

                if (user == null)
                {
                    return NotFound();
                }

                var result = await _userManager.RemoveFromRoleAsync(user, "Admin");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToRoute("Error");
            }
        }

        [HttpGet]
        public IActionResult Roles()
        {
            try
            {
                var roles = _roleManager.Roles.ToList();
                return View(roles);
            }
            catch (Exception ex)
            {
                return RedirectToRoute("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(id);

                if (role == null)
                {
                    ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                    return NotFound();
                }

                var users = await _userManager.GetUsersInRoleAsync(role.Name);

                var usersList = new List<Users>();

                foreach (var user in users)
                {
                    usersList.Add(new Users
                    {
                        Email = user.Email
                    });
                }

                var model = new EditRoleViewModel
                {
                    Id = role.Id,
                    RoleName = role.Name,
                    Users = usersList
                };

                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToRoute("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(model.Id);

                if (role == null)
                {
                    ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found";
                    return NotFound();
                }
                else
                {
                    role.Name = model.RoleName;
                    var result = await _roleManager.UpdateAsync(role);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Roles");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                return RedirectToRoute("Error");
            }
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IdentityRole identityRole = new IdentityRole
                    {
                        Name = model.RoleName
                    };

                    IdentityResult result = await _roleManager.CreateAsync(identityRole);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Roles", "Administration");
                    }

                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToRoute("Error");
            }
        }

        [HttpPost, ActionName("DeleteRole")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return NotFound();
            }
            else
            {
                var users = await _userManager.GetUsersInRoleAsync(role.Name);

                if (users.Count == 0)
                {
                    var result = await _roleManager.DeleteAsync(role);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Roles", "Administration");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                else {
                    ModelState.AddModelError("", "Role has Users, Please remove users from role and try again!");
                }

                return RedirectToAction("Roles", "Administration");
            }
        }

        [HttpGet]
        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> IsEmailInUse([FromQuery(Name = "Input.Email")] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {email} is in Use!");
            }
        }
    }
}
