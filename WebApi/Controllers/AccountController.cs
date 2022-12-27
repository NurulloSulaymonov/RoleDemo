using System.Net;
using Domain.Dtos;
using Domain.Wrapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
// [Authorize]
public class AccountController : ControllerBase
{
    private readonly AccountService _accountService;

    public AccountController(AccountService accountService)
    {
        _accountService = accountService;
    }
    
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<Response<TokenDto>> Login([FromBody] LoginDto loginDto) => await _accountService.Login(loginDto);
    
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<Response<IdentityResult>> Register([FromBody] AddUserDto model) => await _accountService.CreateUser(model);
    
    [HttpPost("assign-user-role")]
    public async  Task<Response<AssignRoleDto>> AssignUserRole([FromBody] AssignRoleDto assignRoleDto) => await _accountService.AssignRole(assignRoleDto);
    
    //remove role
    [HttpPut("remove-user-role")]
    public async Task<Response<AssignRoleDto>> RemoveUserRole([FromBody] AssignRoleDto assignRoleDto) => await _accountService.RemoveRole(assignRoleDto);
    
    [HttpGet("get-user-roles")]
    public async Task<Response<List<string>>> GetUserRoles(string userId) => await _accountService.GetUserRoles(userId);
    
    [HttpGet("get-all-users")]
    public async Task<Response<List<GetUserDto>>> GetAllUsers() => await _accountService.GetAllUsers();
    
    [HttpGet("get-roles")]
    public async Task<Response<List<GetRoleDto>>> GetRoles() => await _accountService.GetAllRoles();
    
}