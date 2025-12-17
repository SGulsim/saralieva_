using Microsoft.EntityFrameworkCore;
using Project.data;
using Project.models;
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
            .Where(p => p.Name.Contains(name))
            .ToListAsync();
    }

    public async Task<IEnumerable<Podcast>> GetByAuthorAsync(string author)
    {
        return await _dbSet
            .Where(p => p.Author.Contains(author))
            .ToListAsync();
    }
}

