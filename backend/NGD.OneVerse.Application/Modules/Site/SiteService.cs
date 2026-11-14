using NGD.OneVerse.Application.Modules.Site.DTOs;
using NGD.OneVerse.Application.Modules.Site.Interfaces;

namespace NGD.OneVerse.Application.Modules.Site;

public sealed class SiteService : ISiteService
{
    private readonly ISiteRepository _repository;

    public SiteService(ISiteRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<SiteDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var sites =
            await _repository.GetAllAsync(cancellationToken);

        return sites
            .OrderBy(x => x.Name)
            .Select(Map)
            .ToList();
    }

    public async Task<SiteDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var site =
            await _repository.GetByIdAsync(
                id,
                cancellationToken);

        return site is null
            ? null
            : Map(site);
    }

    public async Task<SiteDto> CreateAsync(
        CreateSiteRequest request,
        CancellationToken cancellationToken = default)
    {
        Validate(
            request.OrganizationId,
            request.Name,
            request.Code);

        var code = request.Code.Trim();

        if (!await _repository.OrganizationExistsAsync(
                request.OrganizationId,
                cancellationToken))
        {
            throw new InvalidOperationException(
                "Organization does not exist or is inactive.");
        }

        if (await _repository.ExistsByCodeAsync(
                code,
                null,
                cancellationToken))
        {
            throw new InvalidOperationException(
                "Site code already exists.");
        }

        var site = new NGD.OneVerse.Domain.Entities.Site
        {
            Id = Guid.NewGuid(),
            OrganizationId = request.OrganizationId,
            Name = request.Name.Trim(),
            Code = code,
            Description = request.Description?.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(
            site,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return Map(site);
    }

    public async Task<bool> UpdateAsync(
        Guid id,
        UpdateSiteRequest request,
        CancellationToken cancellationToken = default)
    {
        Validate(
            request.OrganizationId,
            request.Name,
            request.Code);

        var site =
            await _repository.GetByIdAsync(
                id,
                cancellationToken);

        if (site is null)
            return false;

        var code = request.Code.Trim();

        if (!await _repository.OrganizationExistsAsync(
                request.OrganizationId,
                cancellationToken))
        {
            throw new InvalidOperationException(
                "Organization does not exist or is inactive.");
        }

        if (await _repository.ExistsByCodeAsync(
                code,
                id,
                cancellationToken))
        {
            throw new InvalidOperationException(
                "Site code already exists.");
        }

        site.OrganizationId = request.OrganizationId;
        site.Name = request.Name.Trim();
        site.Code = code;
        site.Description = request.Description?.Trim();
        site.IsActive = request.IsActive;
        site.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var site =
            await _repository.GetByIdAsync(
                id,
                cancellationToken);

        if (site is null)
            return false;

        _repository.Remove(site);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    private static void Validate(
        Guid organizationId,
        string name,
        string code)
    {
        if (organizationId == Guid.Empty)
            throw new ArgumentException(
                "Organization is required.",
                nameof(organizationId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Site name is required.",
                nameof(name));

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(
                "Site code is required.",
                nameof(code));
    }

    private static SiteDto Map(
        NGD.OneVerse.Domain.Entities.Site site)
    {
        return new SiteDto(
            site.Id,
            site.OrganizationId,
            site.Name,
            site.Code,
            site.Description,
            site.IsActive,
            site.CreatedAt,
            site.UpdatedAt);
    }
}
