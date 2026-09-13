using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SsmsApi.Application.DTOs.Marketplace;
using SsmsApi.Application.Interfaces.Marketplace;

namespace SsmsApi.Api.Controllers.Marketplace;

[ApiController]
[Route("api/market_place/listing-categories")]
public class ListingCategoriesController : ControllerBase
{
    private readonly IListingCategoryService _categoryService;

    public ListingCategoriesController(IListingCategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll() => Ok(await _categoryService.GetAllAsync());

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateListingCategoryRequest request) =>
        Ok(await _categoryService.CreateAsync(request));

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id) =>
        await _categoryService.DeleteAsync(id) ? NoContent() : NotFound();
}