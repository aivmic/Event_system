using Microsoft.AspNetCore.Identity;
using Event_system.Auth.Model;

namespace Event_system.Auth;

public class AuthSeeder
{
    private readonly UserManager<EventUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    
    public AuthSeeder(UserManager<EventUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }
    public async Task SeedAsync()
    {
        await AddDefaultRolesAsync();
        await AddAdminUserAsync();
    }

    private async Task AddAdminUserAsync()
    {
        var newAdminUser = new EventUser()
        {
            UserName = "admin",
            Email = "admin@admin.com"
        };
    
        var existingAdminUser = await _userManager.FindByNameAsync(newAdminUser.UserName);
        if (existingAdminUser == null)
        {
            var createAdminUserResult = await _userManager.CreateAsync(newAdminUser, "VerySafePassword1!");
            if (createAdminUserResult.Succeeded)
            {
                await _userManager.AddToRolesAsync(newAdminUser, EventRoles.All);
            }
        }
    }
    private async Task AddDefaultRolesAsync()
    {
        foreach (var role in EventRoles.All)
        {
            var roleExist = await _roleManager.RoleExistsAsync(role);
            if(!roleExist)
                await _roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}