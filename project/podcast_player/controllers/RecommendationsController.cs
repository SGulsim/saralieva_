using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Project.Services.Interfaces;

namespace Project.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecommendationsController : ControllerBase
{
    private readonly IPodcastService _podcastService;

    public RecommendationsController(IPodcastService podcastService)
    {
        _podcastService = podcastService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Podcast>>> GetRecommendations([FromQuery] int count = 10)
    {
        var recommendations = await _podcastService.GetRecommendedPodcastsAsync(count);
        return Ok(recommendations);
    }
}
