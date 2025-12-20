using AutoMapper;
using Project.Models;
using Project.Repositories.Interfaces;
using Project.Services.Interfaces;

namespace Project.Services;

public class PodcastService : IPodcastService
{
    private readonly IPodcastRepository _repository;
    private readonly IMapper _mapper;
    private readonly IAuthorizationService _authorizationService;

    public PodcastService(IPodcastRepository repository, IMapper mapper, IAuthorizationService authorizationService)
    {
        _repository = repository;
        _mapper = mapper;
        _authorizationService = authorizationService;
    }

    public async Task<IEnumerable<Podcast>> GetAllPodcastsAsync()
    {
        if (!_authorizationService.CanRead("Podcast"))
        {
            throw new UnauthorizedAccessException("Недостаточно прав для чтения подкастов");
        }
        return await _repository.GetAllAsync();
    }

    public async Task<Podcast?> GetPodcastByIdAsync(int id)
    {
        if (!_authorizationService.CanRead("Podcast"))
        {
            throw new UnauthorizedAccessException("Недостаточно прав для чтения подкаста");
        }
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Podcast> CreatePodcastAsync(Podcast podcast)
    {
        if (!_authorizationService.CanCreate("Podcast"))
        {
            throw new UnauthorizedAccessException("Недостаточно прав для создания подкаста");
        }
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

        if (!_authorizationService.CanUpdate("Podcast", null))
        {
            throw new UnauthorizedAccessException("Недостаточно прав для обновления подкаста");
        }

        _mapper.Map(podcast, existingPodcast);

        return await _repository.UpdateAsync(existingPodcast);
    }

    public async Task<bool> DeletePodcastAsync(int id)
    {
        var existingPodcast = await _repository.GetByIdAsync(id);
        if (existingPodcast == null)
        {
            return false;
        }

        if (!_authorizationService.CanDelete("Podcast", null))
        {
            throw new UnauthorizedAccessException("Недостаточно прав для удаления подкаста");
        }

        return await _repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<Podcast>> SearchByNameAsync(string name)
    {
        return await _repository.GetByNameAsync(name);
    }

    public async Task<IEnumerable<Podcast>> GetRecommendedPodcastsAsync(int count = 10)
    {
        var allPodcasts = await _repository.GetAllAsync();
        return allPodcasts
            .Where(p => p.IsFavorite)
            .OrderByDescending(p => p.LastUpdatedAt)
            .Take(count);
    }
}

