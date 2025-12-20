using Project.Models;

namespace Project.Services.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<Category?> GetCategoryByIdAsync(int id);
    Task<Category> CreateCategoryAsync(Category category);
    Task<Category?> UpdateCategoryAsync(int id, Category category);
    Task<bool> DeleteCategoryAsync(int id);
    Task<bool> DeleteCategorySafelyAsync(int id);
    Task<Category?> GetByNameAsync(string name);
}

