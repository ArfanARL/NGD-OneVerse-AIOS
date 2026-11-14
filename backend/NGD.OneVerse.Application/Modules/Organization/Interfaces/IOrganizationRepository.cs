using NGD.OneVerse.Domain.Entities;

namespace NGD.OneVerse.Application.Modules.Organization.Interfaces;

public interface IOrganizationRepository
{
    Task<IReadOnlyList<NGD.OneVerse.Domain.Entities.Organization>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<NGD.OneVerse.Domain.Entities.Organization?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        string code,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        NGD.OneVerse.Domain.Entities.Organization organization,
        CancellationToken cancellationToken = default);

    void Remove(
        NGD.OneVerse.Domain.Entities.Organization organization);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
