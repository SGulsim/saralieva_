using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Project.Services.Interfaces;
using Project.Constants;

namespace Project.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly IPodcastService _podcastService;
    private readonly IEpisodeService _episodeService;

    public SearchController(IPodcastService podcastService, IEpisodeService episodeService)
    {
        _podcastService = podcastService;
        _episodeService = episodeService;
    }

    [HttpGet]
    public async Task<ActionResult<SearchResult>> Search([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest(ErrorMessages.Validation.SearchParameterEmpty);
        }

        var podcasts = await _podcastService.SearchByNameAsync(q);
        var episodes = await _episodeService.SearchByTitleAsync(q);

        return Ok(new SearchResult
        {
            Query = q,
            Podcasts = podcasts.ToList(),
            Episodes = episodes.ToList()
        });
    }
}
