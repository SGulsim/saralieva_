using Project.Models;
using Project.Repositories.Interfaces;
using Project.Services.Interfaces;

namespace Project.Services;

public class PodcastService : IPodcastService
{
    private readonly IPodcastRepository _repository;

    public PodcastService(IPodcastRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Podcast>> GetAllPodcastsAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Podcast?> GetPodcastByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Podcast> CreatePodcastAsync(Podcast podcast)
    {
        podcast.CreatedAt = DateTime.UtcNow;
        podcast.UpdatedAt = DateTime.UtcNow;
        
        return await _repository.AddAsync(podcast);
    }

    public async Task<Podcast?> UpdatePodcastAsync(int id, Podcast podcast)
    {
        var existingPodcast = await _repository.GetByIdAsync(id);
        if (existingPodcast == null)
        {
            return null;
        }

        existingPodcast.Name = podcast.Name;
        existingPodcast.Author = podcast.Author;
        existingPodcast.UpdatedAt = DateTime.UtcNow;

        return await _repository.UpdateAsync(existingPodcast);
    }

    public async Task<bool> DeletePodcastAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }
}

