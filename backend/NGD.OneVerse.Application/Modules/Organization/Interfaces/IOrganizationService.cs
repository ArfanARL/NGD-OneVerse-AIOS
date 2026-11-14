using NGD.OneVerse.Application.Modules.Organization.DTOs;

namespace NGD.OneVerse.Application.Modules.Organization.Interfaces;

public interface IOrganizationService
{
    Task<IReadOnlyList<OrganizationDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<OrganizationDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<OrganizationDto> CreateAsync(
        CreateOrganizationRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        Guid id,
        UpdateOrganizationRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
