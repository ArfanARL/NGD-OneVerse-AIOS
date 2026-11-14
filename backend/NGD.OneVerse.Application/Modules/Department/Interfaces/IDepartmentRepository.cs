namespace NGD.OneVerse.Application.Modules.Department.Interfaces;

public interface IDepartmentRepository
{
    Task<IReadOnlyList<NGD.OneVerse.Domain.Entities.Department>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<NGD.OneVerse.Domain.Entities.Department?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> SiteExistsAsync(
        Guid siteId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        Guid siteId,
        string code,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        NGD.OneVerse.Domain.Entities.Department department,
        CancellationToken cancellationToken = default);

    void Remove(
        NGD.OneVerse.Domain.Entities.Department department);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
