using Project.Models;

namespace Project.Repositories.Interfaces;

public interface IPlaylistRepository : IRepository<Playlist>
{
    Task<IEnumerable<Playlist>> GetByOwnerIdAsync(int ownerId);
}

