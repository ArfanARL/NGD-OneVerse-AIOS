using Microsoft.AspNetCore.Mvc;
using NGD.OneVerse.Application.Modules.Employee.DTOs;
using NGD.OneVerse.Application.Modules.Employee.Interfaces;

namespace NGD.OneVerse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _service;

    public EmployeesController(IEmployeeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EmployeeDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var employees = await _service.GetAllAsync(cancellationToken);
        return Ok(employees);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EmployeeDto>> Get(
        Guid id,
        CancellationToken cancellationToken)
    {
        var employee = await _service.GetByIdAsync(
            id,
            cancellationToken);

        if (employee is null)
            return NotFound();

        return Ok(employee);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> Create(
        [FromBody] CreateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var employee = await _service.CreateAsync(
                request,
                cancellationToken);

            return CreatedAtAction(
                nameof(Get),
                new { id = employee.Id },
                employee);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _service.UpdateAsync(
                id,
                request,
                cancellationToken);

            if (!updated)
                return NotFound();

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteAsync(
            id,
            cancellationToken);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
