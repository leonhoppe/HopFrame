using HopFrame.Security.Models;

namespace HopFrame.Web.Model;

public class RegisterData : UserRegister {
    public string RepeatedPassword { get; set; }
}