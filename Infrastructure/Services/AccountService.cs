using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services;

public class AccountService
{
    private readonly DataContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public AccountService(DataContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    
    
}