using Project.Models;

namespace Project.Services.Interfaces;

public interface IPodcastService
{
    Task<IEnumerable<Podcast>> GetAllPodcastsAsync();
    Task<Podcast?> GetPodcastByIdAsync(int id);
    Task<Podcast> CreatePodcastAsync(Podcast podcast);
    Task<Podcast?> UpdatePodcastAsync(int id, Podcast podcast);
    Task<bool> DeletePodcastAsync(int id);
}

