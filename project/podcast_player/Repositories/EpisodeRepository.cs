using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Repositories.Interfaces;

namespace Project.Repositories;

public class EpisodeRepository : Repository<Episode>, IEpisodeRepository
{
    public EpisodeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Episode>> GetByPodcastIdAsync(int podcastId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(e => e.PodcastId == podcastId)
            .OrderByDescending(e => e.PublishedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Episode>> GetRecentEpisodesAsync(int count = 10)
    {
        return await _dbSet
            .AsNoTracking()
            .OrderByDescending(e => e.PublishedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Episode>> GetUnplayedEpisodesAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Where(e => e.ProgressInSeconds == 0)
            .OrderByDescending(e => e.PublishedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Episode>> GetDownloadedEpisodesAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Where(e => e.IsDownloaded)
            .OrderByDescending(e => e.PublishedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Episode>> SearchByTitleAsync(string title)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(e => e.Title.Contains(title))
            .OrderByDescending(e => e.PublishedAt)
            .ToListAsync();
    }
}

