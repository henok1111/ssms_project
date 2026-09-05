using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SsmsApi.Application.DTOs.Materials;
using SsmsApi.Application.Interfaces;

namespace SsmsApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MaterialItemsController : ControllerBase
{
    private readonly IMaterialItemService _materialItemService;

    public MaterialItemsController(IMaterialItemService materialItemService)
    {
        _materialItemService = materialItemService;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var item = await _materialItemService.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] Guid? categoryId, [FromQuery] string? name)
    {
        var items = await _materialItemService.SearchAsync(categoryId, name);
        return Ok(items);
    }

    [HttpGet("mine")]
    [Authorize(Roles = "Supplier")]
    public async Task<IActionResult> GetMyCatalog()
    {
        var items = await _materialItemService.GetBySupplierAsync(CurrentUserId);
        return Ok(items);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var items = await _materialItemService.GetAllAsync();
        return Ok(items);
    }

    [HttpPost]
    [Authorize(Roles = "Supplier")]
    public async Task<IActionResult> Create([FromBody] CreateMaterialItemRequest request)
    {
        var item = await _materialItemService.CreateAsync(CurrentUserId, request);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Supplier")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMaterialItemRequest request)
    {
        var item = await _materialItemService.UpdateAsync(id, CurrentUserId, request);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Supplier")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _materialItemService.DeleteAsync(id, CurrentUserId);
        return success ? NoContent() : NotFound();
    }
}