using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Repositories.Interfaces;

namespace Project.Repositories;

public class PodcastRepository : Repository<Podcast>, IPodcastRepository
{
    public PodcastRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Podcast>> GetByNameAsync(string name)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(p => p.Name.Contains(name))
            .ToListAsync();
    }

    public async Task<IEnumerable<Podcast>> GetByAuthorAsync(string author)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(p => p.Name.Contains(author))
            .ToListAsync();
    }
}

