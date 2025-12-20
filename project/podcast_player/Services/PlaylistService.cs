using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Repositories.Interfaces;
using Project.Services.Interfaces;

namespace Project.Services;

public class PlaylistService : IPlaylistService
{
    private readonly IPlaylistRepository _repository;
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IAuthorizationService _authorizationService;

    public PlaylistService(IPlaylistRepository repository, ApplicationDbContext context, IMapper mapper, IAuthorizationService authorizationService)
    {
        _repository = repository;
        _context = context;
        _mapper = mapper;
        _authorizationService = authorizationService;
    }

    public async Task<IEnumerable<Playlist>> GetAllPlaylistsAsync()
    {
        if (!_authorizationService.CanRead("Playlist"))
        {
            throw new UnauthorizedAccessException("Недостаточно прав для чтения плейлистов");
        }
        return await _repository.GetAllAsync();
    }

    public async Task<Playlist?> GetPlaylistByIdAsync(int id)
    {
        if (!_authorizationService.CanRead("Playlist"))
        {
            throw new UnauthorizedAccessException("Недостаточно прав для чтения плейлиста");
        }
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Playlist> CreatePlaylistAsync(Playlist playlist)
    {
        if (!_authorizationService.CanCreate("Playlist"))
        {
            throw new UnauthorizedAccessException("Недостаточно прав для создания плейлиста");
        }
        playlist.CreatedAt = DateTime.UtcNow;
        playlist.UpdatedAt = DateTime.UtcNow;
        
        return await _repository.AddAsync(playlist);
    }

    public async Task<Playlist?> UpdatePlaylistAsync(int id, Playlist playlist)
    {
        var existingPlaylist = await _repository.GetByIdAsync(id);
        if (existingPlaylist == null)
        {
            return null;
        }

        if (!_authorizationService.CanUpdate("Playlist", existingPlaylist.OwnerId))
        {
            throw new UnauthorizedAccessException("Недостаточно прав для обновления плейлиста");
        }

        _mapper.Map(playlist, existingPlaylist);

        return await _repository.UpdateAsync(existingPlaylist);
    }

    public async Task<bool> DeletePlaylistAsync(int id)
    {
        var existingPlaylist = await _repository.GetByIdAsync(id);
        if (existingPlaylist == null)
        {
            return false;
        }

        if (!_authorizationService.CanDelete("Playlist", existingPlaylist.OwnerId))
        {
            throw new UnauthorizedAccessException("Недостаточно прав для удаления плейлиста");
        }

        return await _repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<Playlist>> GetByOwnerIdAsync(int ownerId)
    {
        return await _repository.GetByOwnerIdAsync(ownerId);
    }

    public async Task<bool> AddEpisodeToPlaylistAsync(int playlistId, int episodeId)
    {
        var playlist = await _repository.GetByIdAsync(playlistId);
        if (playlist == null)
        {
            return false;
        }

        var episode = await _context.Episodes.FindAsync(episodeId);
        if (episode == null)
        {
            return false;
        }

        var existing = await _context.PlaylistEpisodes
            .FirstOrDefaultAsync(pe => pe.PlaylistId == playlistId && pe.EpisodeId == episodeId);

        if (existing != null)
        {
            return false; // Уже существует
        }

        var maxOrder = await _context.PlaylistEpisodes
            .Where(pe => pe.PlaylistId == playlistId)
            .Select(pe => (int?)pe.Order)
            .MaxAsync() ?? 0;

        var playlistEpisode = new PlaylistEpisode
        {
            PlaylistId = playlistId,
            EpisodeId = episodeId,
            Order = maxOrder + 1,
            AddedAt = DateTime.UtcNow
        };

        await _context.PlaylistEpisodes.AddAsync(playlistEpisode);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveEpisodeFromPlaylistAsync(int playlistId, int episodeId)
    {
        var playlistEpisode = await _context.PlaylistEpisodes
            .FirstOrDefaultAsync(pe => pe.PlaylistId == playlistId && pe.EpisodeId == episodeId);

        if (playlistEpisode == null)
        {
            return false;
        }

        _context.PlaylistEpisodes.Remove(playlistEpisode);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<Episode>> GetPlaylistEpisodesAsync(int playlistId)
    {
        return await _context.PlaylistEpisodes
            .Where(pe => pe.PlaylistId == playlistId)
            .OrderBy(pe => pe.Order)
            .Join(_context.Episodes,
                pe => pe.EpisodeId,
                e => e.Id,
                (pe, e) => e)
            .ToListAsync();
    }
}

