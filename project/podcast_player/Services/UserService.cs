using AutoMapper;
using Project.Models;
using Project.Repositories.Interfaces;
using Project.Services.Interfaces;

namespace Project.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;
    private readonly IAuthorizationService _authorizationService;

    public UserService(IUserRepository repository, IMapper mapper, IAuthorizationService authorizationService)
    {
        _repository = repository;
        _mapper = mapper;
        _authorizationService = authorizationService;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        if (!_authorizationService.CanRead("User"))
        {
            throw new UnauthorizedAccessException("Недостаточно прав для чтения пользователей");
        }
        return await _repository.GetAllAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        if (!_authorizationService.CanRead("User"))
        {
            throw new UnauthorizedAccessException("Недостаточно прав для чтения пользователя");
        }
        return await _repository.GetByIdAsync(id);
    }

    public async Task<User> CreateAsync(User user)
    {
        if (!_authorizationService.CanCreate("User"))
        {
            throw new UnauthorizedAccessException("Недостаточно прав для создания пользователя");
        }
        return await _repository.AddAsync(user);
    }

    public async Task<User?> UpdateAsync(int id, User user)
    {
        if (!_authorizationService.CanUpdate("User", id))
        {
            throw new UnauthorizedAccessException("Недостаточно прав для обновления пользователя");
        }
        var existingUser = await _repository.GetByIdAsync(id);
        if (existingUser == null)
        {
            return null;
        }

        _mapper.Map(user, existingUser);

        // Обновляем пароль только если он был изменен
        if (!string.IsNullOrEmpty(user.PasswordHash) && user.PasswordHash != existingUser.PasswordHash)
        {
            existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
        }

        return await _repository.UpdateAsync(existingUser);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (!_authorizationService.CanDelete("User", id))
        {
            throw new UnauthorizedAccessException("Недостаточно прав для удаления пользователя");
        }
        return await _repository.DeleteAsync(id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _repository.GetByEmailAsync(email);
    }
}

