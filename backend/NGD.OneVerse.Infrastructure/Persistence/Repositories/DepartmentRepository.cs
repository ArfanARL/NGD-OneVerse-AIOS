using Microsoft.EntityFrameworkCore;
using NGD.OneVerse.Application.Modules.Department.Interfaces;
using NGD.OneVerse.Domain.Entities;

namespace NGD.OneVerse.Infrastructure.Persistence.Repositories;

public sealed class DepartmentRepository : IDepartmentRepository
{
    private readonly ApplicationDbContext _db;

    public DepartmentRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Department>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _db.Departments
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<Department?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _db.Departments
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<bool> SiteExistsAsync(
        Guid siteId,
        CancellationToken cancellationToken = default)
    {
        return _db.Sites
            .AsNoTracking()
            .AnyAsync(x => x.Id == siteId && x.IsActive, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(
        Guid siteId,
        string code,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Departments
            .AsNoTracking()
            .Where(x => x.SiteId == siteId && x.Code == code);

        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(
        Department department,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(department);
        await _db.Departments.AddAsync(department, cancellationToken);
    }

    public void Remove(Department department)
    {
        ArgumentNullException.ThrowIfNull(department);
        _db.Departments.Remove(department);
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _db.SaveChangesAsync(cancellationToken);
    }
}