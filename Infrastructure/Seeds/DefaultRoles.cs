using Domain.Constants;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Seeds;

public static class DefaultRoleSeed
{
    public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
    {
        var roles = new List<string>()
        {
            Roles.SuperAdmin,
            Roles.Mentor,
            Roles.Student
        };
        foreach (string role in roles)
        {
            var roleExist = await roleManager.RoleExistsAsync(role);
            if (roleExist == false)
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }  
        } 
    }
}