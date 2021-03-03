using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using CourseWork.Models;

namespace CourseWork.Controllers
{
    [Authorize (Roles ="admin")]
    public class AdminController : Controller
    {
        public RoleManager<IdentityRole> roleManager;
        public UserManager<IdentityUser> userManager;
        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        public IActionResult Index()
        {
            return View(userManager.Users.ToList());
        }
        public async Task<IActionResult> Delete(string id)
        {
            IdentityUser user = await userManager.FindByIdAsync(id);
            if (user != null) await userManager.DeleteAsync(user);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Block(string id)
        {
            IdentityUser user = await userManager.FindByIdAsync(id);
            await userManager.SetLockoutEndDateAsync(user, DateTime.Now.AddYears(1));
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Unblock(string id)
        {
            IdentityUser user = await userManager.FindByIdAsync(id);
            await userManager.SetLockoutEndDateAsync(user, null);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit (string id)
        {
            IdentityUser user = await userManager.FindByIdAsync(id);
            if(user != null)
            {
                UserManagerViewModel model = new UserManagerViewModel { 
                    Email = user.Email, Id = user.Id, AllRoles = roleManager.Roles.ToList(), Roles = await userManager.GetRolesAsync(user)
                };
                return View(model);
            }
            return NotFound();            
        }
        [HttpPost]
        public async Task<IActionResult> Edit(string id, List<string> roles)
        {
            IdentityUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                var previousRoles = await userManager.GetRolesAsync(user);

                await userManager.AddToRolesAsync(user, roles.Except(previousRoles));
                await userManager.RemoveFromRolesAsync(user, previousRoles.Except(roles));

                return RedirectToAction("Index");
            }
            return NotFound();
        }
    }
}
