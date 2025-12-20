using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Project.Services.Interfaces;
using FluentValidation;
using Project.Constants;
using Project.Authorization;

namespace Project.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PlaylistController : ControllerBase
{
    private readonly IPlaylistService _playlistService;
    private readonly IValidator<Playlist> _validator;

    public PlaylistController(IPlaylistService playlistService, IValidator<Playlist> validator)
    {
        _playlistService = playlistService;
        _validator = validator;
    }

    [HttpGet]
    [Authorize(Policy = Permissions.ReadPlaylists)]
    public async Task<ActionResult<IEnumerable<Playlist>>> Get()
    {
        var playlists = await _playlistService.GetAllPlaylistsAsync();
        return Ok(playlists);
    }
    
    [HttpGet("{id}")]
    [Authorize(Policy = Permissions.ReadPlaylists)]
    public async Task<ActionResult<Playlist>> Get(int id)
    {
        var playlist = await _playlistService.GetPlaylistByIdAsync(id);
        
        if (playlist == null)
        {
            return NotFound(string.Format(ErrorMessages.Playlist.NotFoundById, id));
        }
        
        return Ok(playlist);
    }

    [HttpGet("owner/{ownerId}")]
    [Authorize(Policy = Permissions.ReadPlaylists)]
    public async Task<ActionResult<IEnumerable<Playlist>>> GetByOwnerId(int ownerId)
    {
        var playlists = await _playlistService.GetByOwnerIdAsync(ownerId);
        return Ok(playlists);
    }

    [HttpGet("{id}/episodes")]
    [Authorize(Policy = Permissions.ReadPlaylists)]
    public async Task<ActionResult<IEnumerable<Episode>>> GetEpisodes(int id)
    {
        var episodes = await _playlistService.GetPlaylistEpisodesAsync(id);
        return Ok(episodes);
    }

    [HttpPost]
    [Authorize(Policy = Permissions.CreatePlaylists)]
    public async Task<ActionResult<Playlist>> Post([FromBody] Playlist playlist)
    {
        var validationResult = await _validator.ValidateAsync(playlist);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var createdPlaylist = await _playlistService.CreatePlaylistAsync(playlist);
        return CreatedAtAction(nameof(Get), new { id = createdPlaylist.Id }, createdPlaylist);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = Permissions.UpdatePlaylists)]
    public async Task<ActionResult<Playlist>> Put(int id, [FromBody] Playlist playlist)
    {
        if (id != playlist.Id)
        {
            return BadRequest(ErrorMessages.Validation.IdMismatch);
        }

        var validationResult = await _validator.ValidateAsync(playlist);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var updatedPlaylist = await _playlistService.UpdatePlaylistAsync(id, playlist);
        
        if (updatedPlaylist == null)
        {
            return NotFound(string.Format(ErrorMessages.Playlist.NotFoundById, id));
        }

        return Ok(updatedPlaylist);
    }

    [HttpPost("{id}/episodes/{episodeId}")]
    public async Task<ActionResult> AddEpisode(int id, int episodeId)
    {
        var result = await _playlistService.AddEpisodeToPlaylistAsync(id, episodeId);
        
        if (!result)
        {
            return NotFound(string.Format(ErrorMessages.Playlist.NotFoundEpisodeOrPlaylist, id, episodeId));
        }

        return Ok(new { message = "Эпизод успешно добавлен в плейлист" });
    }

    [HttpDelete("{id}/episodes/{episodeId}")]
    public async Task<ActionResult> RemoveEpisode(int id, int episodeId)
    {
        var result = await _playlistService.RemoveEpisodeFromPlaylistAsync(id, episodeId);
        
        if (!result)
        {
            return NotFound(string.Format(ErrorMessages.Playlist.EpisodeNotFoundInPlaylist, episodeId, id));
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = Permissions.DeletePlaylists)]
    public async Task<ActionResult> Delete(int id)
    {
        var deleted = await _playlistService.DeletePlaylistAsync(id);
        
        if (!deleted)
        {
            return NotFound(string.Format(ErrorMessages.Playlist.NotFoundById, id));
        }

        return NoContent();
    }
}

