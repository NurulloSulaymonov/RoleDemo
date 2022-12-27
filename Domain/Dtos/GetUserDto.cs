namespace Domain.Dtos;

public class GetUserDto
{
    public string UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }

    public GetUserDto(string userId, string username, string email)
    {
        UserId = userId;
        Username = username;
        Email = email;
    }
    
}