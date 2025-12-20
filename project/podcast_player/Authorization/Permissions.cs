namespace Project.Authorization;

public static class Permissions
{
    public const string ReadPodcasts = "ReadPodcasts";
    public const string CreatePodcasts = "CreatePodcasts";
    public const string UpdatePodcasts = "UpdatePodcasts";
    public const string DeletePodcasts = "DeletePodcasts";

    public const string ReadEpisodes = "ReadEpisodes";
    public const string CreateEpisodes = "CreateEpisodes";
    public const string UpdateEpisodes = "UpdateEpisodes";
    public const string DeleteEpisodes = "DeleteEpisodes";

    public const string ReadCategories = "ReadCategories";
    public const string CreateCategories = "CreateCategories";
    public const string UpdateCategories = "UpdateCategories";
    public const string DeleteCategories = "DeleteCategories";

    public const string ReadPlaylists = "ReadPlaylists";
    public const string CreatePlaylists = "CreatePlaylists";
    public const string UpdatePlaylists = "UpdatePlaylists";
    public const string DeletePlaylists = "DeletePlaylists";

    public const string ReadUsers = "ReadUsers";
    public const string CreateUsers = "CreateUsers";
    public const string UpdateUsers = "UpdateUsers";
    public const string DeleteUsers = "DeleteUsers";

    public static class Policies
    {
        public const string AdminOnly = "AdminOnly";
        public const string ManagerOrAdmin = "ManagerOrAdmin";
        public const string AllAuthenticated = "AllAuthenticated";
    }
}

