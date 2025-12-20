using Dapper;
using Npgsql;
using Project.Models;
using Project.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Project.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly string _connectionString;
    private readonly ILogger<CategoryRepository> _logger;

    public CategoryRepository(IConfiguration configuration, ILogger<CategoryRepository> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        _logger = logger;
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        const string sql = @"
            SELECT 
                id AS Id,
                name AS Name,
                icon AS Icon,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt
            FROM categories
            WHERE id = @Id";

        await using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<Category>(sql, new { Id = id });
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        const string sql = @"
            SELECT 
                id AS Id,
                name AS Name,
                icon AS Icon,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt
            FROM categories
            ORDER BY name";

        await using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryAsync<Category>(sql);
    }

    public async Task<Category> AddAsync(Category entity)
    {
        const string sql = @"
            INSERT INTO categories (name, icon, created_at, updated_at)
            VALUES (@Name, @Icon, @CreatedAt, @UpdatedAt)
            RETURNING 
                id AS Id,
                name AS Name,
                icon AS Icon,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt";

        await using var connection = new NpgsqlConnection(_connectionString);
        var result = await connection.QueryFirstOrDefaultAsync<Category>(sql, new
        {
            entity.Name,
            entity.Icon,
            CreatedAt = entity.CreatedAt == default ? DateTime.UtcNow : entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt == default ? DateTime.UtcNow : entity.UpdatedAt
        });

        if (result == null)
        {
            throw new InvalidOperationException("Failed to insert category.");
        }

        return result;
    }

    public async Task<Category> UpdateAsync(Category entity)
    {
        const string sql = @"
            UPDATE categories
            SET name = @Name,
                icon = @Icon,
                updated_at = @UpdatedAt
            WHERE id = @Id
            RETURNING 
                id AS Id,
                name AS Name,
                icon AS Icon,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt";

        await using var connection = new NpgsqlConnection(_connectionString);
        var result = await connection.QueryFirstOrDefaultAsync<Category>(sql, new
        {
            entity.Id,
            entity.Name,
            entity.Icon,
            UpdatedAt = DateTime.UtcNow
        });

        if (result == null)
        {
            throw new InvalidOperationException($"Category with id {entity.Id} not found.");
        }

        return result;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = @"
            DELETE FROM categories
            WHERE id = @Id";

        await using var connection = new NpgsqlConnection(_connectionString);
        var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
        return affectedRows > 0;
    }

    public async Task<bool> DeleteSafelyAsync(int id)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        await using var transaction = await connection.BeginTransactionAsync();
        
        try
        {
            const string checkSql = @"
                SELECT COUNT(*) 
                FROM podcasts 
                WHERE category_id = @CategoryId";
            
            var podcastCount = await connection.QueryFirstOrDefaultAsync<int>(
                checkSql, 
                new { CategoryId = id }, 
                transaction);

            if (podcastCount > 0)
            {
                const string updateSql = @"
                    UPDATE podcasts 
                    SET category_id = NULL 
                    WHERE category_id = @CategoryId";
                
                await connection.ExecuteAsync(
                    updateSql, 
                    new { CategoryId = id }, 
                    transaction);
            }

            const string deleteSql = @"
                DELETE FROM categories 
                WHERE id = @Id";
            
            var affectedRows = await connection.ExecuteAsync(
                deleteSql, 
                new { Id = id }, 
                transaction);

            await transaction.CommitAsync();
            
            _logger.LogInformation(
                "Транзакция успешно завершена: удалена категория {CategoryId}, обновлено подкастов: {PodcastCount}",
                id,
                podcastCount);
            
            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Ошибка при безопасном удалении категории {CategoryId}: {Message}. Транзакция откачена.",
                id,
                ex.Message);
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        const string sql = @"
            SELECT EXISTS(SELECT 1 FROM categories WHERE id = @Id)";

        await using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<bool>(sql, new { Id = id });
    }

    public async Task<Category?> GetByNameAsync(string name)
    {
        const string sql = @"
            SELECT 
                id AS Id,
                name AS Name,
                icon AS Icon,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt
            FROM categories
            WHERE name = @Name";

        await using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<Category>(sql, new { Name = name });
    }
}
