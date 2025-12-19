using Project.Models;

namespace Project.Repositories.Interfaces;

public interface IPodcastRepository : IRepository<Podcast>
{
    Task<IEnumerable<Podcast>> GetByNameAsync(string name);
    Task<IEnumerable<Podcast>> GetByAuthorAsync(string author);
}

