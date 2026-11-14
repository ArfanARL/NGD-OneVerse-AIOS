using NGD.OneVerse.Domain.Entities;

namespace NGD.OneVerse.Application.Modules.Site.Interfaces;

public interface ISiteRepository
{
    Task<IReadOnlyList<NGD.OneVerse.Domain.Entities.Site>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<NGD.OneVerse.Domain.Entities.Site?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        string code,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    Task<bool> OrganizationExistsAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        NGD.OneVerse.Domain.Entities.Site site,
        CancellationToken cancellationToken = default);

    void Remove(
        NGD.OneVerse.Domain.Entities.Site site);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
