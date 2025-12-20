using AutoMapper;
using Project.Models;
using Project.Repositories.Interfaces;
using Project.Services.Interfaces;

namespace Project.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<CategoryService> _logger;
    private readonly IAuthorizationService _authorizationService;

    public CategoryService(ICategoryRepository repository, IMapper mapper, ILogger<CategoryService> logger, IAuthorizationService authorizationService)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
        _authorizationService = authorizationService;
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        if (!_authorizationService.CanRead("Category"))
        {
            throw new UnauthorizedAccessException("Недостаточно прав для чтения категорий");
        }
        _logger.LogInformation("Бизнес-событие: Запрос на получение всех категорий");
        var categories = await _repository.GetAllAsync();
        _logger.LogInformation("Бизнес-событие: Получено {Count} категорий", categories.Count());
        return categories;
    }

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        if (!_authorizationService.CanRead("Category"))
        {
            throw new UnauthorizedAccessException("Недостаточно прав для чтения категории");
        }
        _logger.LogInformation("Бизнес-событие: Запрос категории по ID {CategoryId}", id);
        var category = await _repository.GetByIdAsync(id);
        if (category == null)
        {
            _logger.LogWarning("Бизнес-событие: Категория с ID {CategoryId} не найдена", id);
        }
        return category;
    }

    public async Task<Category> CreateCategoryAsync(Category category)
    {
        if (!_authorizationService.CanCreate("Category"))
        {
            throw new UnauthorizedAccessException("Недостаточно прав для создания категории");
        }
        _logger.LogInformation("Бизнес-событие: Создание новой категории с именем {CategoryName}", category.Name);
        category.CreatedAt = DateTime.UtcNow;
        category.UpdatedAt = DateTime.UtcNow;
        
        var createdCategory = await _repository.AddAsync(category);
        _logger.LogInformation("Бизнес-событие: Категория успешно создана с ID {CategoryId}", createdCategory.Id);
        return createdCategory;
    }

    public async Task<Category?> UpdateCategoryAsync(int id, Category category)
    {
        if (!_authorizationService.CanUpdate("Category", null))
        {
            throw new UnauthorizedAccessException("Недостаточно прав для обновления категории");
        }
        _logger.LogInformation("Бизнес-событие: Обновление категории с ID {CategoryId}", id);
        var existingCategory = await _repository.GetByIdAsync(id);
        if (existingCategory == null)
        {
            _logger.LogWarning("Бизнес-событие: Попытка обновления несуществующей категории с ID {CategoryId}", id);
            return null;
        }

        _mapper.Map(category, existingCategory);

        var updatedCategory = await _repository.UpdateAsync(existingCategory);
        _logger.LogInformation("Бизнес-событие: Категория с ID {CategoryId} успешно обновлена", id);
        return updatedCategory;
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        if (!_authorizationService.CanDelete("Category", null))
        {
            throw new UnauthorizedAccessException("Недостаточно прав для удаления категории");
        }
        _logger.LogInformation("Бизнес-событие: Удаление категории с ID {CategoryId}", id);
        var result = await _repository.DeleteAsync(id);
        if (result)
        {
            _logger.LogInformation("Бизнес-событие: Категория с ID {CategoryId} успешно удалена", id);
        }
        else
        {
            _logger.LogWarning("Бизнес-событие: Не удалось удалить категорию с ID {CategoryId}", id);
        }
        return result;
    }

    public async Task<bool> DeleteCategorySafelyAsync(int id)
    {
        if (!_authorizationService.CanDelete("Category", null))
        {
            throw new UnauthorizedAccessException("Недостаточно прав для удаления категории");
        }
        _logger.LogInformation("Бизнес-событие: Безопасное удаление категории с ID {CategoryId} (с транзакцией)", id);
        var result = await _repository.DeleteSafelyAsync(id);
        if (result)
        {
            _logger.LogInformation("Бизнес-событие: Категория с ID {CategoryId} успешно удалена безопасным способом", id);
        }
        else
        {
            _logger.LogWarning("Бизнес-событие: Не удалось безопасно удалить категорию с ID {CategoryId}", id);
        }
        return result;
    }

    public async Task<Category?> GetByNameAsync(string name)
    {
        _logger.LogInformation("Бизнес-событие: Поиск категории по имени {CategoryName}", name);
        return await _repository.GetByNameAsync(name);
    }
}

