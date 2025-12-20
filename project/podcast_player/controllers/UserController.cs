using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Project.Services.Interfaces;
using FluentValidation;
using Project.Constants;
using Project.Authorization;

namespace Project.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
    [Authorize(Policy = Permissions.ReadUsers)]
    public async Task<ActionResult<IEnumerable<User>>> Get()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }
    
    [HttpGet("{id}")]
    [Authorize(Policy = Permissions.ReadUsers)]
    public async Task<ActionResult<User>> Get(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        
        if (user == null)
        {
            return NotFound(string.Format(ErrorMessages.User.NotFoundById, id));
        }
        
        return Ok(user);
    }

    [HttpPost]
    [Authorize(Policy = Permissions.CreateUsers)]
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
    [Authorize(Policy = Permissions.UpdateUsers)]
    public async Task<ActionResult<User>> Put(int id, [FromBody] User user)
    {
        if (id != user.Id)
        {
            return BadRequest(ErrorMessages.Validation.IdMismatch);
        }

        var validationResult = await _validator.ValidateAsync(user);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var updatedUser = await _userService.UpdateAsync(id, user);
        
        if (updatedUser == null)
        {
            return NotFound(string.Format(ErrorMessages.User.NotFoundById, id));
        }

        return Ok(updatedUser);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = Permissions.DeleteUsers)]
    public async Task<ActionResult> Delete(int id)
    {
        var deleted = await _userService.DeleteAsync(id);
        
        if (!deleted)
        {
            return NotFound(string.Format(ErrorMessages.User.NotFoundById, id));
        }

        return NoContent();
    }
}
