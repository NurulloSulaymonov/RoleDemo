using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Domain.Dtos;
using Domain.Wrapper;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public class AccountService
{
    private readonly DataContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountService(DataContext context, 
        UserManager<IdentityUser> userManager, 
        IConfiguration configuration,
        RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _configuration = configuration;
        _roleManager = roleManager;
    }

    public async Task<Response<IdentityResult>> CreateUser(AddUserDto userDto)
    {
        try
        {
            var user = new IdentityUser()
            {
                Email = userDto.Email,
                UserName = userDto.Username,
            };
            var response = await _userManager.CreateAsync(user, userDto.Password);
            if (response.Succeeded  == true)
            {
                return new Response<IdentityResult>(response);
            }
            return new Response<IdentityResult>(HttpStatusCode.BadRequest, $"{String.Join(",", response.Errors.Select(x => x.Description))}");
        }
        catch (Exception ex)
        {
            return new Response<IdentityResult>(HttpStatusCode.InternalServerError, ex.Message);
        }
    } 
    
    public async Task<Response<TokenDto>> Login(LoginDto loginDto)
    {
        try
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);
            if (user == null)
            {
                return new Response<TokenDto>(HttpStatusCode.NotFound, "User not found");
            }
            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (result == false)
            {
                return new Response<TokenDto>(HttpStatusCode.BadRequest, "Invalid password");
            }
            var token = await GenerateJwtToken(user);
            return new Response<TokenDto>(token);
        }
        catch (Exception ex)
        {
            return new Response<TokenDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }
    
    private async Task<TokenDto> GenerateJwtToken(IdentityUser user)
    {
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
        var securityKey = new SymmetricSecurityKey(key);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
        };
        //add roles
        
        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role=>new Claim(ClaimTypes.Role,role)));
        
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        var securityTokenHandler = new JwtSecurityTokenHandler();
        var tokenString = securityTokenHandler.WriteToken(token);
        return new TokenDto()
        {
            Token = tokenString
        };
    }
    
    public async Task<Response<AssignRoleDto>> AssignRole(AssignRoleDto assignRoleDto)
    {
        var user = await _userManager.FindByIdAsync(assignRoleDto.UserId);
        if (user == null)
        {
            return new Response<AssignRoleDto>(HttpStatusCode.NotFound, "User not found");
        }
        var role = await _roleManager.FindByIdAsync(assignRoleDto.RoleId);
        if (role == null)
        {
            return new Response<AssignRoleDto>(HttpStatusCode.NotFound, "Role not found");
        }
        var result = await _userManager.AddToRoleAsync(user, role.Name);
        if (result.Succeeded == false)
        {
            return new Response<AssignRoleDto>(HttpStatusCode.BadRequest, $"{String.Join(",", result.Errors.Select(x => x.Description))}");
        }
        return new Response<AssignRoleDto>(assignRoleDto);
    }
    
    public async Task<Response<AssignRoleDto>> RemoveRole(AssignRoleDto assignRoleDto)
    {
        var user = await _userManager.FindByIdAsync(assignRoleDto.UserId);
        if (user == null)
        {
            return new Response<AssignRoleDto>(HttpStatusCode.NotFound, "User not found");
        }
        var role = await _roleManager.FindByIdAsync(assignRoleDto.RoleId);
        if (role == null)
        {
            return new Response<AssignRoleDto>(HttpStatusCode.NotFound, "Role not found");
        }
        var result = await _userManager.RemoveFromRoleAsync(user, role.Name);
        if (!result.Succeeded)
        {
            return new Response<AssignRoleDto>(HttpStatusCode.BadRequest, $"{String.Join(",", result.Errors.Select(x => x.Description))}");
        }
        return new Response<AssignRoleDto>(assignRoleDto);
    }
    
    //get user roles
    public async Task<Response<List<string>>> GetUserRoles(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new Response<List<string>>(HttpStatusCode.NotFound, "User not found");
        }
        var roles = await _userManager.GetRolesAsync(user);
        return new Response<List<string>>(roles.ToList());
    }
    
    //get all roles
    public async Task<Response<List<GetRoleDto>>> GetAllRoles()
    {
        var roles = await _roleManager.Roles.Select(x=> new GetRoleDto(x.Id,x.Name)).ToListAsync();
        return new Response<List<GetRoleDto>>(roles.ToList());
    }
    
    // get all users
    public async Task<Response<List<GetUserDto>>> GetAllUsers()
    {
        var users = await _userManager.Users.Select(x=> new GetUserDto(x.Email,x.UserName,x.Id)).ToListAsync();
        return new Response<List<GetUserDto>>(users.ToList());
    }
}