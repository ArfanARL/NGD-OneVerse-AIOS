using Microsoft.EntityFrameworkCore;
using NGD.OneVerse.Application.Modules.Organization.Interfaces;
using NGD.OneVerse.Domain.Entities;

namespace NGD.OneVerse.Infrastructure.Persistence.Repositories;

public sealed class OrganizationRepository : IOrganizationRepository
{
    private readonly ApplicationDbContext _db;

    public OrganizationRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Organization>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _db.Organizations
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Organization?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _db.Organizations
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(
        string code,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Organizations
            .AsNoTracking()
            .Where(x => x.Code == code);

        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(
        Organization organization,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(organization);

        await _db.Organizations.AddAsync(
            organization,
            cancellationToken);
    }

    public void Remove(Organization organization)
    {
        ArgumentNullException.ThrowIfNull(organization);

        _db.Organizations.Remove(organization);
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _db.SaveChangesAsync(cancellationToken);
    }
}
