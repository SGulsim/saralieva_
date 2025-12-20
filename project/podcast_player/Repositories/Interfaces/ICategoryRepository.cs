using Project.Models;

namespace Project.Repositories.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetByNameAsync(string name);
    Task<bool> DeleteSafelyAsync(int id);
}

