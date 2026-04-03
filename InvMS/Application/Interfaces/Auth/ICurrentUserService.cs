namespace Application.Interfaces.Auth;

/// <summary>
/// Provides the identity of the currently authenticated user.
/// Implemented in Infrastructure using IHttpContextAccessor to read JWT claims.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Returns the Username of the logged-in user (from ClaimTypes.Name in JWT).
    /// Returns null if no user is authenticated (e.g., during seeding).
    /// </summary>
    string? Username { get; }
}
