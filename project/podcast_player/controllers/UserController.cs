using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Project.Services.Interfaces;
using FluentValidation;

namespace Project.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IValidator<User> _validator;

    public UserController(IUserService userService, IValidator<User> validator)
    {
        _userService = userService;
        _validator = validator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> Get()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> Get(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        
        if (user == null)
        {
            return NotFound($"Пользователь с id {id} не найден");
        }
        
        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<User>> Post([FromBody] User user)
    {
        var validationResult = await _validator.ValidateAsync(user);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
        
        var createdUser = await _userService.CreateAsync(user);
        return CreatedAtAction(nameof(Get), new { id = createdUser.Id }, createdUser);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<User>> Put(int id, [FromBody] User user)
    {
        if (id != user.Id)
        {
            return BadRequest("ID в URL не совпадает с ID в теле запроса");
        }

        var validationResult = await _validator.ValidateAsync(user);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var updatedUser = await _userService.UpdateAsync(id, user);
        
        if (updatedUser == null)
        {
            return NotFound($"Пользователь с id {id} не найден");
        }

        return Ok(updatedUser);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var deleted = await _userService.DeleteAsync(id);
        
        if (!deleted)
        {
            return NotFound($"Пользователь с id {id} не найден");
        }

        return NoContent();
    }
}
