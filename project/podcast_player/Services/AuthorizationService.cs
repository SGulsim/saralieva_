using System.Security.Claims;
using Project.Models;
using Project.Authorization;

namespace Project.Services;

public interface IAuthorizationService
{
    bool CanRead(string resourceType);
    bool CanCreate(string resourceType);
    bool CanUpdate(string resourceType, int? resourceOwnerId = null);
    bool CanDelete(string resourceType, int? resourceOwnerId = null);
    int? GetCurrentUserId(ClaimsPrincipal user);
    Role? GetCurrentUserRole(ClaimsPrincipal user);
}

public class AuthorizationService : IAuthorizationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthorizationService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool CanRead(string resourceType)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null || !user.Identity?.IsAuthenticated == true)
            return false;

        var role = GetCurrentUserRole(user);
        if (role == null)
            return false;

        // Все роли могут читать
        return role == Role.User || role == Role.Manager || role == Role.Admin;
    }

    public bool CanCreate(string resourceType)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null || !(user.Identity?.IsAuthenticated == true))
            return false;

        var role = GetCurrentUserRole(user);
        if (role == null)
            return false;

        // Playlists: все могут создавать
        if (resourceType == "Playlist")
            return true;

        // Остальное: Manager и Admin
        return role == Role.Manager || role == Role.Admin;
    }

    public bool CanUpdate(string resourceType, int? resourceOwnerId = null)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null || !(user.Identity?.IsAuthenticated == true))
            return false;

        var role = GetCurrentUserRole(user);
        if (role == null)
            return false;

        var currentUserId = GetCurrentUserId(user);

        // Admin может обновлять все
        if (role == Role.Admin)
            return true;

        // Manager может обновлять все
        if (role == Role.Manager)
            return true;

        // User может обновлять только свои ресурсы
        if (role == Role.User && resourceOwnerId.HasValue && currentUserId.HasValue)
            return resourceOwnerId.Value == currentUserId.Value;

        return false;
    }

    public bool CanDelete(string resourceType, int? resourceOwnerId = null)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null || !(user.Identity?.IsAuthenticated == true))
            return false;

        var role = GetCurrentUserRole(user);
        if (role == null)
            return false;

        var currentUserId = GetCurrentUserId(user);

        // Admin может удалять все
        if (role == Role.Admin)
            return true;

        // Manager может удалять только свои ресурсы
        if (role == Role.Manager && resourceOwnerId.HasValue && currentUserId.HasValue)
            return resourceOwnerId.Value == currentUserId.Value;

        // User может удалять только свои ресурсы
        if (role == Role.User && resourceOwnerId.HasValue && currentUserId.HasValue)
            return resourceOwnerId.Value == currentUserId.Value;

        return false;
    }

    public int? GetCurrentUserId(ClaimsPrincipal user)
    {
        var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? user?.FindFirst("UserId")?.Value;
        
        if (int.TryParse(userIdClaim, out var userId))
            return userId;
        
        return null;
    }

    public Role? GetCurrentUserRole(ClaimsPrincipal user)
    {
        var roleClaim = user?.FindFirst(ClaimTypes.Role)?.Value
            ?? user?.FindFirst("Role")?.Value;
        
        if (Enum.TryParse<Role>(roleClaim, out var role))
            return role;
        
        return null;
    }
}

