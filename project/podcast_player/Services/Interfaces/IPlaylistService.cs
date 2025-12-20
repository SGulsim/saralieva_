using Project.Models;

namespace Project.Services.Interfaces;

public interface IPlaylistService
{
    Task<IEnumerable<Playlist>> GetAllPlaylistsAsync();
    Task<Playlist?> GetPlaylistByIdAsync(int id);
    Task<Playlist> CreatePlaylistAsync(Playlist playlist);
    Task<Playlist?> UpdatePlaylistAsync(int id, Playlist playlist);
    Task<bool> DeletePlaylistAsync(int id);
    Task<IEnumerable<Playlist>> GetByOwnerIdAsync(int ownerId);
    Task<bool> AddEpisodeToPlaylistAsync(int playlistId, int episodeId);
    Task<bool> RemoveEpisodeFromPlaylistAsync(int playlistId, int episodeId);
    Task<IEnumerable<Episode>> GetPlaylistEpisodesAsync(int playlistId);
}

