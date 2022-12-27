using System.Net;
using Domain.Dtos;
using Domain.Wrapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;

    public AccountController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }
    
    [HttpPost]
    public async Task<Response<IdentityResult>> Register([FromBody] AddUserDto model)
    {
        var user = new IdentityUser
        {
            UserName = model.Email,
            Email = model.Email
        };
        var result = await _userManager.CreateAsync(user, model.Password);
        if(result.Succeeded)
        {
            return new Response<IdentityResult>(result);
        }
        return new Response<IdentityResult>(HttpStatusCode.BadRequest, "Login or password is wrong");
    }
    
    
}