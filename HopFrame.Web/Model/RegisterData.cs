using HopFrame.Security.Models;

namespace HopFrame.Web.Model;

internal class RegisterData : UserRegister {
    public string RepeatedPassword { get; set; }

    public bool PasswordsMatch => Password == RepeatedPassword;
    public bool PasswordIsValid => Password?.Length >= 8;
    public bool EmailIsValid => Email?.Contains('@') == true && Email?.Contains('.') == true && Email?.EndsWith('.') == false;
}