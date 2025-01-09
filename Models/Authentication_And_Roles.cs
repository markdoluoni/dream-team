//Handle role classes for authorization

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;

namespace CommunityManager.Models
{
    //Class for user
    public class CommunityUser : IdentityUser
    {

        public int? ResidentID { get; set; }
        //Put any needed properties in here, but check the og IdentityUser class for inherited data
    }

    public class SeedData
    {
        public static async Task Initialize(CommunityContext context, UserManager<CommunityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();
            /*
                An admin page will let us directly assign roles

                Resident should be the default assignment
            */

            //Create the roles
            var roleNames = new[] {"Resident", "Manager" };
            
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            
            
            /* Optionally, create a user and assign a role:
            var adminUser = await userManager.FindByNameAsync("AdminExample");
            if (adminUser == null)
            {
                adminUser = new CommunityUser
                {
                    UserName = "AdminExample",
                };
                var Result = await userManager.CreateAsync(adminUser, "1293-AdminPass-4893$%^");
            }

            if (!await userManager.IsInRoleAsync(adminUser, "Manager"))
            {
                await userManager.AddToRoleAsync(adminUser, "Manager");
            }*/
        }
    }

}