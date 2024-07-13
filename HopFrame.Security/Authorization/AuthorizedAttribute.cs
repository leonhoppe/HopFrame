using Microsoft.AspNetCore.Mvc;

namespace HopFrame.Security.Authorization;

public class AuthorizedAttribute : TypeFilterAttribute {
    public AuthorizedAttribute(params string[] permission) : base(typeof(AuthorizedFilter)) {
        Arguments = new object[] { permission };
    }
}