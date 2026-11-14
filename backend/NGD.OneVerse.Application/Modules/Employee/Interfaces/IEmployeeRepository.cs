using NGD.OneVerse.Domain.Entities;

namespace NGD.OneVerse.Application.Modules.Employee.Interfaces;

public interface IEmployeeRepository
{
    Task<IReadOnlyList<NGD.OneVerse.Domain.Entities.Employee>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<NGD.OneVerse.Domain.Entities.Employee?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmployeeNumberAsync(
        string employeeNumber,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmailAsync(
        string email,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    Task<bool> OrganizationExistsAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);

    Task<bool> DepartmentExistsAsync(
        Guid departmentId,
        CancellationToken cancellationToken = default);

    Task<bool> SiteExistsAsync(
        Guid siteId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        NGD.OneVerse.Domain.Entities.Employee employee,
        CancellationToken cancellationToken = default);

    void Remove(
        NGD.OneVerse.Domain.Entities.Employee employee);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
