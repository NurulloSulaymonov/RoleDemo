namespace Domain.Dtos;

public class GetRoleDto
{
    public string RoleId { get; set; }
    public string Name { get; set; }

    public GetRoleDto(string roleId, string name)
    {
        RoleId = roleId;
        Name = name;
    }
    
}