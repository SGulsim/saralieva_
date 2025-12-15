using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.data;
using Project.models;

namespace Project.controllers;

[ApiController]
[Route(template:"podcast")]
public class PodcastControllers: ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PodcastControllers(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Podcast>>> Get()
    {
        var podcasts = await _context.Podcasts.ToListAsync();
        return Ok(podcasts);
    }
    
    [HttpGet(template:"{id}")]
    public async Task<ActionResult<Podcast>> Get(int id)
    {
        var podcast = await _context.Podcasts.FindAsync(id);
        
        if (podcast == null)
        {
            return NotFound($"Podcast with id {id} not found");
        }
        
        return Ok(podcast);
    }

    [HttpPost]
    public async Task<ActionResult<Podcast>> Post([FromBody] Podcast podcast)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        podcast.CreatedAt = DateTime.UtcNow;
        podcast.UpdatedAt = DateTime.UtcNow;

        _context.Podcasts.Add(podcast);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = podcast.Id }, podcast);
    }

    [HttpPut(template: "{id}")]
    public async Task<ActionResult<Podcast>> Put(int id, [FromBody] Podcast podcast)
    {
        if (id != podcast.Id)
        {
            return BadRequest("Id mismatch");
        }

        var existingPodcast = await _context.Podcasts.FindAsync(id);
        if (existingPodcast == null)
        {
            return NotFound($"Podcast with id {id} not found");
        }

        existingPodcast.Name = podcast.Name;
        existingPodcast.Author = podcast.Author;
        existingPodcast.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Podcasts.AnyAsync(e => e.Id == id))
            {
                return NotFound();
            }
            throw;
        }

        return Ok(existingPodcast);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var podcast = await _context.Podcasts.FindAsync(id);
        if (podcast == null)
        {
            return NotFound($"Podcast with id {id} not found");
        }

        _context.Podcasts.Remove(podcast);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}