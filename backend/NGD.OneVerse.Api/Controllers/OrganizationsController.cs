using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NGD.OneVerse.Domain.Entities;
using NGD.OneVerse.Infrastructure.Persistence;

namespace NGD.OneVerse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrganizationsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public OrganizationsController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Organization>>> GetOrganizations()
    {
        var organizations = await _db.Organizations
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();

        return Ok(organizations);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Organization>> GetOrganization(Guid id)
    {
        var organization = await _db.Organizations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (organization is null)
            return NotFound();

        return Ok(organization);
    }

    [HttpPost]
    public async Task<ActionResult<Organization>> CreateOrganization(
        Organization organization)
    {
        if (string.IsNullOrWhiteSpace(organization.Name))
            return BadRequest("Organization name is required.");

        if (string.IsNullOrWhiteSpace(organization.Code))
            return BadRequest("Organization code is required.");

        var codeExists = await _db.Organizations
            .AnyAsync(x => x.Code == organization.Code);

        if (codeExists)
            return Conflict("Organization code already exists.");

        organization.Id = Guid.NewGuid();
        organization.CreatedAt = DateTime.UtcNow;
        organization.UpdatedAt = null;
        organization.IsActive = true;

        _db.Organizations.Add(organization);

        await _db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetOrganization),
            new { id = organization.Id },
            organization);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateOrganization(
        Guid id,
        Organization organization)
    {
        if (id != organization.Id)
            return BadRequest("ID mismatch.");

        var existing = await _db.Organizations
            .FirstOrDefaultAsync(x => x.Id == id);

        if (existing is null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(organization.Name))
            return BadRequest("Organization name is required.");

        if (string.IsNullOrWhiteSpace(organization.Code))
            return BadRequest("Organization code is required.");

        var codeExists = await _db.Organizations
            .AnyAsync(x => x.Code == organization.Code && x.Id != id);

        if (codeExists)
            return Conflict("Organization code already exists.");

        existing.Name = organization.Name;
        existing.Code = organization.Code;
        existing.Description = organization.Description;
        existing.IsActive = organization.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteOrganization(Guid id)
    {
        var organization = await _db.Organizations
            .FirstOrDefaultAsync(x => x.Id == id);

        if (organization is null)
            return NotFound();

        _db.Organizations.Remove(organization);

        await _db.SaveChangesAsync();

        return NoContent();
    }
}