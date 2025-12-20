using Project.Models;

namespace Project.Repositories.Interfaces;

public interface IEpisodeRepository : IRepository<Episode>
{
    Task<IEnumerable<Episode>> GetByPodcastIdAsync(int podcastId);
    Task<IEnumerable<Episode>> GetRecentEpisodesAsync(int count = 10);
    Task<IEnumerable<Episode>> GetUnplayedEpisodesAsync();
    Task<IEnumerable<Episode>> GetDownloadedEpisodesAsync();
    Task<IEnumerable<Episode>> SearchByTitleAsync(string title);
}

