namespace HopFrame.Api.Models;

public class UserCreator {
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public virtual List<string> Permissions { get; set; }
}