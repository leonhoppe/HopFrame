using HopFrame.Database;
using HopFrame.Database.Models;
using Microsoft.AspNetCore.Http;

namespace HopFrame.Security.Claims;

public class TokenContextImplementor : ITokenContextBase {
    private readonly IHttpContextAccessor _accessor;
    private readonly HopDbContextBase _context;

    public TokenContextImplementor(IHttpContextAccessor accessor, HopDbContextBase context) {
        _accessor = accessor;
        _context = context;
    }

    public bool IsAuthenticated => _accessor.HttpContext?.User.Identity?.IsAuthenticated == true;

    public User User => _context.Users
        .SingleOrDefault(user => user.Id == _accessor.HttpContext.User.GetUserId())?
        .ToUserModel(_context);
    
    public Guid AccessToken => Guid.Parse(_accessor.HttpContext?.User.GetAccessTokenId() ?? string.Empty);
}