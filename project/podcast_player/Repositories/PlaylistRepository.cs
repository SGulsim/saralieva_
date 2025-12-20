using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Repositories.Interfaces;

namespace Project.Repositories;

public class PlaylistRepository : Repository<Playlist>, IPlaylistRepository
{
    public PlaylistRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Playlist>> GetByOwnerIdAsync(int ownerId)
    {
        return await _dbSet
            .Where(p => p.OwnerId == ownerId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }
}

