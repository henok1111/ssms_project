using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SsmsApi.Application.Interfaces;
using SsmsApi.Domain.Enums;

namespace SsmsApi.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("workers/pending")]
    public async Task<IActionResult> GetPendingWorkers() =>
        Ok(await _adminService.GetPendingWorkersAsync());

    [HttpGet("suppliers/pending")]
    public async Task<IActionResult> GetPendingSuppliers() =>
        Ok(await _adminService.GetPendingSuppliersAsync());

    [HttpPost("workers/{id:guid}/approve")]
    public async Task<IActionResult> ApproveWorker(Guid id) =>
        await _adminService.ApproveWorkerAsync(id) ? Ok(new { message = "Worker approved." }) : NotFound();

    [HttpPost("workers/{id:guid}/reject")]
    public async Task<IActionResult> RejectWorker(Guid id) =>
        await _adminService.RejectWorkerAsync(id) ? Ok(new { message = "Worker rejected." }) : NotFound();

    [HttpPost("suppliers/{id:guid}/approve")]
    public async Task<IActionResult> ApproveSupplier(Guid id) =>
        await _adminService.ApproveSupplierAsync(id) ? Ok(new { message = "Supplier approved." }) : NotFound();

    [HttpPost("suppliers/{id:guid}/reject")]
    public async Task<IActionResult> RejectSupplier(Guid id) =>
        await _adminService.RejectSupplierAsync(id) ? Ok(new { message = "Supplier rejected." }) : NotFound();

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] UserRole? role)
    {
        var users = await _adminService.GetUsersAsync(role);
        return Ok(users);
    }

    [HttpPost("users/{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id) =>
        await _adminService.DeactivateUserAsync(id) ? Ok(new { message = "User deactivated." }) : NotFound();

    [HttpPost("users/{id:guid}/reactivate")]
    public async Task<IActionResult> Reactivate(Guid id) =>
        await _adminService.ReactivateUserAsync(id) ? Ok(new { message = "User reactivated." }) : NotFound();
}