using Microsoft.AspNetCore.Mvc;
using NGD.OneVerse.Application.Modules.Department.DTOs;
using NGD.OneVerse.Application.Modules.Department.Interfaces;

namespace NGD.OneVerse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _service;

    public DepartmentsController(IDepartmentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DepartmentDto>>> GetDepartments(
        CancellationToken cancellationToken)
    {
        return Ok(await _service.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DepartmentDto>> GetDepartment(
        Guid id,
        CancellationToken cancellationToken)
    {
        var department = await _service.GetByIdAsync(id, cancellationToken);
        return department is null ? NotFound() : Ok(department);
    }

    [HttpPost]
    public async Task<ActionResult<DepartmentDto>> CreateDepartment(
        CreateDepartmentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var created = await _service.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetDepartment), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateDepartment(
        Guid id,
        UpdateDepartmentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, request, cancellationToken);
            return updated ? NoContent() : NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteDepartment(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}