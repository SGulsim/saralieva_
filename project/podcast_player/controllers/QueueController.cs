using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Project.Services.Interfaces;

namespace Project.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QueueController : ControllerBase
{
    private readonly IEpisodeService _episodeService;

    public QueueController(IEpisodeService episodeService)
    {
        _episodeService = episodeService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Episode>>> GetQueue()
    {
        var queue = await _episodeService.GetUnplayedEpisodesAsync();
        return Ok(queue.OrderByDescending(e => e.PublishedAt));
    }
}
