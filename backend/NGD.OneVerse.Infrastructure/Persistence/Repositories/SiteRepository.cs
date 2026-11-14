using Microsoft.EntityFrameworkCore;
using NGD.OneVerse.Application.Modules.Site.Interfaces;
using NGD.OneVerse.Domain.Entities;

namespace NGD.OneVerse.Infrastructure.Persistence.Repositories;

public sealed class SiteRepository : ISiteRepository
{
    private readonly ApplicationDbContext _db;

    public SiteRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Site>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _db.Sites
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Site?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _db.Sites
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(
        string code,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Sites
            .AsNoTracking()
            .Where(x => x.Code == code);

        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public Task<bool> OrganizationExistsAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        return _db.Organizations
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == organizationId && x.IsActive,
                cancellationToken);
    }

    public async Task AddAsync(
        Site site,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(site);

        await _db.Sites.AddAsync(site, cancellationToken);
    }

    public void Remove(Site site)
    {
        ArgumentNullException.ThrowIfNull(site);

        _db.Sites.Remove(site);
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _db.SaveChangesAsync(cancellationToken);
    }
}
