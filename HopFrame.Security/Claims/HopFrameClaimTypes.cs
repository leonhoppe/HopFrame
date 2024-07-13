using System.Security.Claims;

namespace HopFrame.Security.Claims;

public static class HopFrameClaimTypes {
    public const string AccessTokenId = "HopFrame.AccessTokenId";
    public const string UserId = "HopFrame.UserId";
    public const string Permission = "HopFrame.Permission";

    public static string GetAccessTokenId(this ClaimsPrincipal principal) {
        return principal.FindFirstValue(AccessTokenId);
    }
    
    public static string GetUserId(this ClaimsPrincipal principal) {
        return principal.FindFirstValue(UserId);
    }
    
    public static string[] GetPermissions(this ClaimsPrincipal principal) {
        return principal.FindAll(Permission).Select(claim => claim.Value).ToArray();
    }
}