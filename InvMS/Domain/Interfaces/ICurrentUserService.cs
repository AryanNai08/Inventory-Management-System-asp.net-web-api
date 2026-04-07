using Domain.Interfaces;

namespace Domain.Interfaces;

/// <summary>
/// Provides the identity of the currently authenticated user.
/// Implemented in Infrastructure using IHttpContextAccessor to read JWT claims.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Returns the Unique Identifier (Username) for auditing purposes.
    /// </summary>
    string? UserId { get; }

    /// <summary>
    /// Returns the Username of the logged-in user (from ClaimTypes.Name in JWT).
    /// </summary>
    string? Username { get; }
}
