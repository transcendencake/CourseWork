using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CourseWork.Models;

namespace CourseWork
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if(!await roleManager.RoleExistsAsync("admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }
            if(!await roleManager.RoleExistsAsync("user"))
            {
                await roleManager.CreateAsync(new IdentityRole("user"));
            }
            if (!await roleManager.RoleExistsAsync("blocked"))
            {
                await roleManager.CreateAsync(new IdentityRole("blocked"));
            }
            var adminEmail = "admin@tut.by";
            var adminPassword = "123456Qw.";
            if(await userManager.FindByEmailAsync(adminEmail) == null)
            {
                ApplicationUser user = new ApplicationUser { Email = adminEmail, UserName = adminEmail };
                IdentityResult result = await userManager.CreateAsync(user, adminPassword);
                foreach (var error in result.Errors)
                {
                    Logger.DebugLogger.LogError(error.ToString());
                }
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "admin");
                }
            }

        }
    }
}
