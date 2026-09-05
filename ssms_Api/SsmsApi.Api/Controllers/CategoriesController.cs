using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SsmsApi.Application.DTOs.Categories;
using SsmsApi.Application.Interfaces;

namespace SsmsApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    [AllowAnonymous] // anyone browsing jobs/materials needs to see categories, even logged out
    public async Task<IActionResult> GetAll([FromQuery] bool? isServiceCategory)
    {
        var categories = isServiceCategory.HasValue
            ? await _categoryService.GetByTypeAsync(isServiceCategory.Value)
            : await _categoryService.GetAllAsync();

        return Ok(categories);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        return category is null ? NotFound() : Ok(category);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
    {
        var category = await _categoryService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequest request)
    {
        var category = await _categoryService.UpdateAsync(id, request);
        return category is null ? NotFound() : Ok(category);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _categoryService.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }
}