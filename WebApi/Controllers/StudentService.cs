using Domain.Constants;
using Domain.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
// [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Student},{Roles.Mentor}")]
[Route("[controller]")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class StudentService:ControllerBase
{
    public StudentService()
    {
        
    }
    
    [HttpGet("everyone")]
    [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Student},{Roles.Mentor}")]
    public Response<string> Get()
    {
        return new Response<string>("Hello World from StudentService");
    }
    
    [HttpGet("OnlyTeacher")]
    [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Mentor}")]
    public Response<string> OnlyTeacher()
    {
        return new Response<string>("Hello World from admin and teacher");
    }
    
    [HttpGet("OnlyAdmin")]
    [Authorize(Roles = $"{Roles.SuperAdmin}")]
    public Response<string> OnlyAdmin()
    {
        return new Response<string>("Hello World from Admin");
    }
    
}