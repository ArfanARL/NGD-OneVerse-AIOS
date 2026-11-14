using NGD.OneVerse.Application.Modules.Site.DTOs;

namespace NGD.OneVerse.Application.Modules.Site.Interfaces;

public interface ISiteService
{
    Task<IReadOnlyList<SiteDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<SiteDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<SiteDto> CreateAsync(
        CreateSiteRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        Guid id,
        UpdateSiteRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
