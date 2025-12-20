using AutoMapper;
using Project.Models;
using Project.Repositories.Interfaces;
using Project.Services.Interfaces;

namespace Project.Services;

public class EpisodeService : IEpisodeService
{
    private readonly IEpisodeRepository _repository;
    private readonly IMapper _mapper;
    private readonly IAuthorizationService _authorizationService;

    public EpisodeService(IEpisodeRepository repository, IMapper mapper, IAuthorizationService authorizationService)
    {
        _repository = repository;
        _mapper = mapper;
        _authorizationService = authorizationService;
    }

    public async Task<IEnumerable<Episode>> GetAllEpisodesAsync()
    {
        if (!_authorizationService.CanRead("Episode"))
        {
            throw new UnauthorizedAccessException("Недостаточно прав для чтения эпизодов");
        }
        return await _repository.GetAllAsync();
    }

    public async Task<Episode?> GetEpisodeByIdAsync(int id)
    {
        if (!_authorizationService.CanRead("Episode"))
        {
            throw new UnauthorizedAccessException("Недостаточно прав для чтения эпизода");
        }
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Episode> CreateEpisodeAsync(Episode episode)
    {
        if (!_authorizationService.CanCreate("Episode"))
        {
            throw new UnauthorizedAccessException("Недостаточно прав для создания эпизода");
        }
        episode.CreatedAt = DateTime.UtcNow;
        episode.UpdatedAt = DateTime.UtcNow;
        
        return await _repository.AddAsync(episode);
    }

    public async Task<Episode?> UpdateEpisodeAsync(int id, Episode episode)
    {
        var existingEpisode = await _repository.GetByIdAsync(id);
        if (existingEpisode == null)
        {
            return null;
        }

        if (!_authorizationService.CanUpdate("Episode", null))
        {
            throw new UnauthorizedAccessException("Недостаточно прав для обновления эпизода");
        }

        _mapper.Map(episode, existingEpisode);

        return await _repository.UpdateAsync(existingEpisode);
    }

    public async Task<bool> DeleteEpisodeAsync(int id)
    {
        var existingEpisode = await _repository.GetByIdAsync(id);
        if (existingEpisode == null)
        {
            return false;
        }

        if (!_authorizationService.CanDelete("Episode", null))
        {
            throw new UnauthorizedAccessException("Недостаточно прав для удаления эпизода");
        }

        return await _repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<Episode>> GetByPodcastIdAsync(int podcastId)
    {
        return await _repository.GetByPodcastIdAsync(podcastId);
    }

    public async Task<IEnumerable<Episode>> GetRecentEpisodesAsync(int count = 10)
    {
        return await _repository.GetRecentEpisodesAsync(count);
    }

    public async Task<IEnumerable<Episode>> GetUnplayedEpisodesAsync()
    {
        return await _repository.GetUnplayedEpisodesAsync();
    }

    public async Task<IEnumerable<Episode>> GetDownloadedEpisodesAsync()
    {
        return await _repository.GetDownloadedEpisodesAsync();
    }

    public async Task<Episode?> UpdateProgressAsync(int id, int progressInSeconds)
    {
        var episode = await _repository.GetByIdAsync(id);
        if (episode == null)
        {
            return null;
        }

        episode.ProgressInSeconds = progressInSeconds;
        episode.UpdatedAt = DateTime.UtcNow;

        return await _repository.UpdateAsync(episode);
    }

    public async Task<Episode?> MarkAsDownloadedAsync(int id, bool isDownloaded)
    {
        var episode = await _repository.GetByIdAsync(id);
        if (episode == null)
        {
            return null;
        }

        episode.IsDownloaded = isDownloaded;
        episode.UpdatedAt = DateTime.UtcNow;

        return await _repository.UpdateAsync(episode);
    }

    public async Task<IEnumerable<Episode>> SearchByTitleAsync(string title)
    {
        return await _repository.SearchByTitleAsync(title);
    }
}

