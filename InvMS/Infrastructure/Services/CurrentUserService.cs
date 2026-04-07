
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Services;

/// <summary>
/// Reads the currently authenticated user's identity from the HTTP context.
/// The JWT middleware populates HttpContext.User with the claims from the token,
/// so we can read ClaimTypes.Name (Username) directly here.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Returns the Username stored in the JWT token's ClaimTypes.Name claim.
    /// Your AuthService sets this as: new Claim(ClaimTypes.Name, user.Username)
    /// Returns null if no user is logged in (e.g., during seeding or background tasks).
    /// </summary>
    public string? Username =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);

    /// <summary>
    /// Returns the UserId (Username) for auditing purposes.
    /// maps to the same claim for simplicity.
    /// </summary>
    public string? UserId => Username;
}
