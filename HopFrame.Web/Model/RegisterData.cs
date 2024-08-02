using HopFrame.Security.Models;

namespace HopFrame.Web.Model;

public class RegisterData : UserRegister {
    public string RepeatedPassword { get; set; }

    public bool PasswordsMatch => Password == RepeatedPassword;
    public bool PasswordIsValid => Password.Length >= 8;
    public bool EmailIsValid => Email.Contains('@') && Email.Contains('.') && !Email.EndsWith('.');
}