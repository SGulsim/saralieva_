using Microsoft.AspNetCore.Authorization;
using Project.Models;

namespace Project.Authorization;

public static class AuthorizationPolicies
{
    public static void ConfigurePolicies(AuthorizationOptions options)
    {
        // Admin: полный доступ ко всему
        options.AddPolicy(Permissions.Policies.AdminOnly, policy =>
            policy.RequireRole(Role.Admin.ToString()));

        // Manager и Admin: чтение и создание/обновление
        options.AddPolicy(Permissions.Policies.ManagerOrAdmin, policy =>
            policy.RequireRole(Role.Manager.ToString(), Role.Admin.ToString()));

        // Все аутентифицированные пользователи
        options.AddPolicy(Permissions.Policies.AllAuthenticated, policy =>
            policy.RequireAuthenticatedUser());

        // Политики для конкретных операций
        options.AddPolicy(Permissions.ReadPodcasts, policy =>
            policy.RequireAuthenticatedUser());
        options.AddPolicy(Permissions.CreatePodcasts, policy =>
            policy.RequireRole(Role.Manager.ToString(), Role.Admin.ToString()));
        options.AddPolicy(Permissions.UpdatePodcasts, policy =>
            policy.RequireRole(Role.Manager.ToString(), Role.Admin.ToString()));
        options.AddPolicy(Permissions.DeletePodcasts, policy =>
            policy.RequireRole(Role.Admin.ToString()));

        options.AddPolicy(Permissions.ReadEpisodes, policy =>
            policy.RequireAuthenticatedUser());
        options.AddPolicy(Permissions.CreateEpisodes, policy =>
            policy.RequireRole(Role.Manager.ToString(), Role.Admin.ToString()));
        options.AddPolicy(Permissions.UpdateEpisodes, policy =>
            policy.RequireRole(Role.Manager.ToString(), Role.Admin.ToString()));
        options.AddPolicy(Permissions.DeleteEpisodes, policy =>
            policy.RequireRole(Role.Admin.ToString()));

        options.AddPolicy(Permissions.ReadCategories, policy =>
            policy.RequireAuthenticatedUser());
        options.AddPolicy(Permissions.CreateCategories, policy =>
            policy.RequireRole(Role.Manager.ToString(), Role.Admin.ToString()));
        options.AddPolicy(Permissions.UpdateCategories, policy =>
            policy.RequireRole(Role.Manager.ToString(), Role.Admin.ToString()));
        options.AddPolicy(Permissions.DeleteCategories, policy =>
            policy.RequireRole(Role.Admin.ToString()));

        options.AddPolicy(Permissions.ReadPlaylists, policy =>
            policy.RequireAuthenticatedUser());
        options.AddPolicy(Permissions.CreatePlaylists, policy =>
            policy.RequireAuthenticatedUser());
        options.AddPolicy(Permissions.UpdatePlaylists, policy =>
            policy.RequireAuthenticatedUser());
        options.AddPolicy(Permissions.DeletePlaylists, policy =>
            policy.RequireAuthenticatedUser());

        options.AddPolicy(Permissions.ReadUsers, policy =>
            policy.RequireRole(Role.Manager.ToString(), Role.Admin.ToString()));
        options.AddPolicy(Permissions.CreateUsers, policy =>
            policy.RequireRole(Role.Admin.ToString()));
        options.AddPolicy(Permissions.UpdateUsers, policy =>
            policy.RequireRole(Role.Admin.ToString()));
        options.AddPolicy(Permissions.DeleteUsers, policy =>
            policy.RequireRole(Role.Admin.ToString()));
    }
}

