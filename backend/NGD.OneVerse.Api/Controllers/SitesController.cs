using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NGD.OneVerse.Domain.Entities;
using NGD.OneVerse.Infrastructure.Persistence;

namespace NGD.OneVerse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SitesController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public SitesController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Site>>> GetSites()
    {
        return await _db.Sites
            .AsNoTracking()
            .ToListAsync();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Site>> GetSite(Guid id)
    {
        var site = await _db.Sites
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (site == null)
            return NotFound();

        return site;
    }

    [HttpPost]
    public async Task<ActionResult<Site>> CreateSite(Site site)
    {
        if (!await _db.Organizations
            .AnyAsync(x => x.Id == site.OrganizationId))
        {
            return BadRequest("Organization does not exist.");
        }

        site.Id = Guid.NewGuid();
        site.CreatedAt = DateTime.UtcNow;

        _db.Sites.Add(site);
        await _db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetSite),
            new { id = site.Id },
            site);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateSite(
        Guid id,
        Site site)
    {
        if (id != site.Id)
            return BadRequest();

        var existing = await _db.Sites.FindAsync(id);

        if (existing == null)
            return NotFound();

        existing.OrganizationId = site.OrganizationId;
        existing.Name = site.Name;
        existing.Code = site.Code;
        existing.SiteType = site.SiteType;
        existing.Address = site.Address;
        existing.City = site.City;
        existing.Country = site.Country;
        existing.IsActive = site.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteSite(Guid id)
    {
        var site = await _db.Sites.FindAsync(id);

        if (site == null)
            return NotFound();

        _db.Sites.Remove(site);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}