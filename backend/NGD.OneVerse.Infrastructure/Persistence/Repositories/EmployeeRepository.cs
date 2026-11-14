using Microsoft.EntityFrameworkCore;
using NGD.OneVerse.Application.Modules.Employee.Interfaces;
using NGD.OneVerse.Domain.Entities;

namespace NGD.OneVerse.Infrastructure.Persistence.Repositories;

public sealed class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _db;

    public EmployeeRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<NGD.OneVerse.Domain.Entities.Employee>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _db.Employees
            .AsNoTracking()
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task<NGD.OneVerse.Domain.Entities.Employee?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _db.Employees
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<bool> ExistsByEmployeeNumberAsync(
        string employeeNumber,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Employees
            .AsNoTracking()
            .Where(x => x.EmployeeNumber == employeeNumber);

        if (excludeId.HasValue)
        {
            query = query.Where(
                x => x.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(
        string email,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Employees
            .AsNoTracking()
            .Where(x => x.Email == email);

        if (excludeId.HasValue)
        {
            query = query.Where(
                x => x.Id != excludeId.Value);
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
                x => x.Id == organizationId &&
                     x.IsActive,
                cancellationToken);
    }

    public Task<bool> DepartmentExistsAsync(
        Guid departmentId,
        CancellationToken cancellationToken = default)
    {
        return _db.Departments
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == departmentId &&
                     x.IsActive,
                cancellationToken);
    }

    public Task<bool> SiteExistsAsync(
        Guid siteId,
        CancellationToken cancellationToken = default)
    {
        return _db.Sites
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == siteId &&
                     x.IsActive,
                cancellationToken);
    }

    public async Task AddAsync(
        NGD.OneVerse.Domain.Entities.Employee employee,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(employee);

        await _db.Employees.AddAsync(
            employee,
            cancellationToken);
    }

    public void Remove(
        NGD.OneVerse.Domain.Entities.Employee employee)
    {
        ArgumentNullException.ThrowIfNull(employee);

        _db.Employees.Remove(employee);
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _db.SaveChangesAsync(cancellationToken);
    }
}
