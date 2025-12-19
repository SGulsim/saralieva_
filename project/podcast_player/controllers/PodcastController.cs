using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Project.Services.Interfaces;
using FluentValidation;

namespace Project.Controllers;

[ApiController]
[Route(template:"api/[controller]")]
public class PodcastController: ControllerBase
{
    private readonly IPodcastService _podcastService;
    private readonly IValidator<Podcast> _validator;

    public PodcastController(IPodcastService podcastService, IValidator<Podcast> validator)
    {
        _podcastService = podcastService;
        _validator = validator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Podcast>>> Get()
    {
        var podcasts = await _podcastService.GetAllPodcastsAsync();
        return Ok(podcasts);
    }
    
    [HttpGet(template:"{id}")]
    public async Task<ActionResult<Podcast>> Get(int id)
    {
        var podcast = await _podcastService.GetPodcastByIdAsync(id);
        
        if (podcast == null)
        {
            return NotFound($"Podcast с id {id} не найден");
        }
        
        return Ok(podcast);
    }

    [HttpPost]
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
    public async Task<ActionResult<Podcast>> Put(int id, [FromBody] Podcast podcast)
    {
        if (id != podcast.Id)
        {
            return BadRequest("ID в URL не совпадает с ID в теле запроса");
        }

        var validationResult = await _validator.ValidateAsync(podcast);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var updatedPodcast = await _podcastService.UpdatePodcastAsync(id, podcast);
        
        if (updatedPodcast == null)
        {
            return NotFound($"Подкаст с id {id} не найден");
        }

        return Ok(updatedPodcast);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var deleted = await _podcastService.DeletePodcastAsync(id);
        
        if (!deleted)
        {
            return NotFound($"Подкаст с id {id} не найден");
        }

        return NoContent();
    }
}
