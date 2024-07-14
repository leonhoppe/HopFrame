using Microsoft.AspNetCore.Mvc;

namespace HopFrame.Security.Authorization;

public class AuthorizedAttribute : TypeFilterAttribute {
    
    /// <summary>
    /// If this decorator is present, the endpoint is only accessible if the user provided a valid access token (is logged in)
    /// permission system:<br/>
    /// - "*" -> all rights<br/>
    /// - "group.[name]" -> group member<br/>
    /// - "[namespace].[name]" -> single permission<br/>
    /// - "[namespace].*" -> all permissions in the namespace
    /// </summary>
    /// <param name="permissions">specifies the permissions the user needs to have in order to access this endpoint</param>
    public AuthorizedAttribute(params string[] permissions) : base(typeof(AuthorizedFilter)) {
        Arguments = [permissions];
    }
}