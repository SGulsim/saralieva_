using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Project.Services.Interfaces;
using FluentValidation;
using Project.Constants;
using Project.Authorization;

namespace Project.Controllers;

[ApiController]
[Route(template:"api/[controller]")]
[Authorize]
public class PodcastController: ControllerBase
{
    private readonly IPodcastService _podcastService;
    private readonly IEpisodeService _episodeService;
    private readonly IValidator<Podcast> _validator;

    public PodcastController(IPodcastService podcastService, IEpisodeService episodeService, IValidator<Podcast> validator)
    {
        _podcastService = podcastService;
        _episodeService = episodeService;
        _validator = validator;
    }

    [HttpGet]
    [Authorize(Policy = Permissions.ReadPodcasts)]
    public async Task<ActionResult<IEnumerable<Podcast>>> Get()
    {
        var podcasts = await _podcastService.GetAllPodcastsAsync();
        return Ok(podcasts);
    }
    
    [HttpGet(template:"{id}")]
    [Authorize(Policy = Permissions.ReadPodcasts)]
    public async Task<ActionResult<Podcast>> Get(int id)
    {
        var podcast = await _podcastService.GetPodcastByIdAsync(id);
        
        if (podcast == null)
        {
            return NotFound(string.Format(ErrorMessages.Podcast.NotFoundById, id));
        }
        
        return Ok(podcast);
    }

    [HttpGet("{id}/episodes")]
    [Authorize(Policy = Permissions.ReadEpisodes)]
    public async Task<ActionResult<IEnumerable<Episode>>> GetEpisodes(int id)
    {
        var podcast = await _podcastService.GetPodcastByIdAsync(id);
        if (podcast == null)
        {
            return NotFound(string.Format(ErrorMessages.Podcast.NotFoundByIdRu, id));
        }

        var episodes = await _episodeService.GetByPodcastIdAsync(id);
        return Ok(episodes);
    }

    [HttpPost]
    [Authorize(Policy = Permissions.CreatePodcasts)]
    public async Task<ActionResult<Podcast>> Post([FromBody] Podcast podcast)
    {
        var validationResult = await _validator.ValidateAsync(podcast);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var createdPodcast = await _podcastService.CreatePodcastAsync(podcast);
        return CreatedAtAction(nameof(Get), new { id = createdPodcast.Id }, createdPodcast);
    }

    [HttpPut(template: "{id}")]
    [Authorize(Policy = Permissions.UpdatePodcasts)]
    public async Task<ActionResult<Podcast>> Put(int id, [FromBody] Podcast podcast)
    {
        if (id != podcast.Id)
        {
            return BadRequest(ErrorMessages.Validation.IdMismatch);
        }

        var validationResult = await _validator.ValidateAsync(podcast);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var updatedPodcast = await _podcastService.UpdatePodcastAsync(id, podcast);
        
        if (updatedPodcast == null)
        {
            return NotFound(string.Format(ErrorMessages.Podcast.NotFoundByIdRu, id));
        }

        return Ok(updatedPodcast);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = Permissions.DeletePodcasts)]
    public async Task<ActionResult> Delete(int id)
    {
        var deleted = await _podcastService.DeletePodcastAsync(id);
        
        if (!deleted)
        {
            return NotFound(string.Format(ErrorMessages.Podcast.NotFoundByIdRu, id));
        }

        return NoContent();
    }
}
