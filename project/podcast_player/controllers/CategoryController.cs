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
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly IValidator<Category> _validator;

    public CategoryController(ICategoryService categoryService, IValidator<Category> validator)
    {
        _categoryService = categoryService;
        _validator = validator;
    }

    [HttpGet]
    [Authorize(Policy = Permissions.ReadCategories)]
    public async Task<ActionResult<IEnumerable<Category>>> Get()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }
    
    [HttpGet("{id}")]
    [Authorize(Policy = Permissions.ReadCategories)]
    public async Task<ActionResult<Category>> Get(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        
        if (category == null)
        {
            return NotFound(string.Format(ErrorMessages.Category.NotFoundById, id));
        }
        
        return Ok(category);
    }

    [HttpPost]
    [Authorize(Policy = Permissions.CreateCategories)]
    public async Task<ActionResult<Category>> Post([FromBody] Category category)
    {
        var validationResult = await _validator.ValidateAsync(category);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var createdCategory = await _categoryService.CreateCategoryAsync(category);
        return CreatedAtAction(nameof(Get), new { id = createdCategory.Id }, createdCategory);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = Permissions.UpdateCategories)]
    public async Task<ActionResult<Category>> Put(int id, [FromBody] Category category)
    {
        if (id != category.Id)
        {
            return BadRequest(ErrorMessages.Validation.IdMismatch);
        }

        var validationResult = await _validator.ValidateAsync(category);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var updatedCategory = await _categoryService.UpdateCategoryAsync(id, category);
        
        if (updatedCategory == null)
        {
            return NotFound(string.Format(ErrorMessages.Category.NotFoundById, id));
        }

        return Ok(updatedCategory);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = Permissions.DeleteCategories)]
    public async Task<ActionResult> Delete(int id)
    {
        var deleted = await _categoryService.DeleteCategoryAsync(id);
        
        if (!deleted)
        {
            return NotFound(string.Format(ErrorMessages.Category.NotFoundById, id));
        }

        return NoContent();
    }

    [HttpDelete("{id}/safely")]
    [Authorize(Policy = Permissions.DeleteCategories)]
    public async Task<ActionResult> DeleteSafely(int id)
    {
        var deleted = await _categoryService.DeleteCategorySafelyAsync(id);
        
        if (!deleted)
        {
            return NotFound(string.Format(ErrorMessages.Category.NotFoundById, id));
        }

        return NoContent();
    }
}

