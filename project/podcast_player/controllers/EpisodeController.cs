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
public class EpisodeController : ControllerBase
{
    private readonly IEpisodeService _episodeService;
    private readonly IValidator<Episode> _validator;

    public EpisodeController(IEpisodeService episodeService, IValidator<Episode> validator)
    {
        _episodeService = episodeService;
        _validator = validator;
    }

    [HttpGet]
    [Authorize(Policy = Permissions.ReadEpisodes)]
    public async Task<ActionResult<IEnumerable<Episode>>> Get()
    {
        var episodes = await _episodeService.GetAllEpisodesAsync();
        return Ok(episodes);
    }
    
    [HttpGet("{id}")]
    [Authorize(Policy = Permissions.ReadEpisodes)]
    public async Task<ActionResult<Episode>> Get(int id)
    {
        var episode = await _episodeService.GetEpisodeByIdAsync(id);
        
        if (episode == null)
        {
            return NotFound(string.Format(ErrorMessages.Episode.NotFoundById, id));
        }
        
        return Ok(episode);
    }

    [HttpGet("podcast/{podcastId}")]
    [Authorize(Policy = Permissions.ReadEpisodes)]
    public async Task<ActionResult<IEnumerable<Episode>>> GetByPodcastId(int podcastId)
    {
        var episodes = await _episodeService.GetByPodcastIdAsync(podcastId);
        return Ok(episodes);
    }

    [HttpGet("recent")]
    [Authorize(Policy = Permissions.ReadEpisodes)]
    public async Task<ActionResult<IEnumerable<Episode>>> GetRecent([FromQuery] int count = 10)
    {
        var episodes = await _episodeService.GetRecentEpisodesAsync(count);
        return Ok(episodes);
    }

    [HttpGet("unplayed")]
    [Authorize(Policy = Permissions.ReadEpisodes)]
    public async Task<ActionResult<IEnumerable<Episode>>> GetUnplayed()
    {
        var episodes = await _episodeService.GetUnplayedEpisodesAsync();
        return Ok(episodes);
    }

    [HttpGet("downloaded")]
    [Authorize(Policy = Permissions.ReadEpisodes)]
    public async Task<ActionResult<IEnumerable<Episode>>> GetDownloaded()
    {
        var episodes = await _episodeService.GetDownloadedEpisodesAsync();
        return Ok(episodes);
    }

    [HttpGet("search")]
    [Authorize(Policy = Permissions.ReadEpisodes)]
    public async Task<ActionResult<IEnumerable<Episode>>> Search([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest(ErrorMessages.Validation.SearchParameterEmpty);
        }

        var episodes = await _episodeService.SearchByTitleAsync(q);
        return Ok(episodes);
    }

    [HttpPost]
    [Authorize(Policy = Permissions.CreateEpisodes)]
    public async Task<ActionResult<Episode>> Post([FromBody] Episode episode)
    {
        var validationResult = await _validator.ValidateAsync(episode);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var createdEpisode = await _episodeService.CreateEpisodeAsync(episode);
        return CreatedAtAction(nameof(Get), new { id = createdEpisode.Id }, createdEpisode);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = Permissions.UpdateEpisodes)]
    public async Task<ActionResult<Episode>> Put(int id, [FromBody] Episode episode)
    {
        if (id != episode.Id)
        {
            return BadRequest(ErrorMessages.Validation.IdMismatch);
        }

        var validationResult = await _validator.ValidateAsync(episode);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var updatedEpisode = await _episodeService.UpdateEpisodeAsync(id, episode);
        
        if (updatedEpisode == null)
        {
            return NotFound(string.Format(ErrorMessages.Episode.NotFoundById, id));
        }

        return Ok(updatedEpisode);
    }

    [HttpPut("{id}/progress")]
    public async Task<ActionResult<Episode>> UpdateProgress(int id, [FromBody] int progressInSeconds)
    {
        var updatedEpisode = await _episodeService.UpdateProgressAsync(id, progressInSeconds);
        
        if (updatedEpisode == null)
        {
            return NotFound(string.Format(ErrorMessages.Episode.NotFoundById, id));
        }

        return Ok(updatedEpisode);
    }

    [HttpPost("{id}/download")]
    public async Task<ActionResult<Episode>> MarkAsDownloaded(int id, [FromBody] bool isDownloaded)
    {
        var updatedEpisode = await _episodeService.MarkAsDownloadedAsync(id, isDownloaded);
        
        if (updatedEpisode == null)
        {
            return NotFound(string.Format(ErrorMessages.Episode.NotFoundById, id));
        }

        return Ok(updatedEpisode);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = Permissions.DeleteEpisodes)]
    public async Task<ActionResult> Delete(int id)
    {
        var deleted = await _episodeService.DeleteEpisodeAsync(id);
        
        if (!deleted)
        {
            return NotFound(string.Format(ErrorMessages.Episode.NotFoundById, id));
        }

        return NoContent();
    }
}

