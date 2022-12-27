using Domain.Constants;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Seeds;

public static class DefaultUser
{
    public static async Task SeedAsync(UserManager<IdentityUser> userManager)
    {
        var user = new IdentityUser()
        {
            Email = "admin@gmail.com",
            UserName = "admin",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true
        };
        var existing = await userManager.FindByNameAsync(user.UserName);
        if (existing == null)
        { 
            await userManager.CreateAsync(user, "Admin123!");
            await userManager.AddToRoleAsync(user, Roles.SuperAdmin);
        }
        
    }
}