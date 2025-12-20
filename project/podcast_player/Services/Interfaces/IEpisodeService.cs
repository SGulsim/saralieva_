using Project.Models;

namespace Project.Services.Interfaces;

public interface IEpisodeService
{
    Task<IEnumerable<Episode>> GetAllEpisodesAsync();
    Task<Episode?> GetEpisodeByIdAsync(int id);
    Task<Episode> CreateEpisodeAsync(Episode episode);
    Task<Episode?> UpdateEpisodeAsync(int id, Episode episode);
    Task<bool> DeleteEpisodeAsync(int id);
    Task<IEnumerable<Episode>> GetByPodcastIdAsync(int podcastId);
    Task<IEnumerable<Episode>> GetRecentEpisodesAsync(int count = 10);
    Task<IEnumerable<Episode>> GetUnplayedEpisodesAsync();
    Task<IEnumerable<Episode>> GetDownloadedEpisodesAsync();
    Task<Episode?> UpdateProgressAsync(int id, int progressInSeconds);
    Task<Episode?> MarkAsDownloadedAsync(int id, bool isDownloaded);
    Task<IEnumerable<Episode>> SearchByTitleAsync(string title);
}

