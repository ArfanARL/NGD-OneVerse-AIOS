using NGD.OneVerse.Application.Modules.Organization.DTOs;
using NGD.OneVerse.Application.Modules.Organization.Interfaces;
using NGD.OneVerse.Domain.Entities;

namespace NGD.OneVerse.Application.Modules.Organization;

public sealed class OrganizationService : IOrganizationService
{
    private readonly IOrganizationRepository _repository;

    public OrganizationService(IOrganizationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<OrganizationDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var organizations =
            await _repository.GetAllAsync(cancellationToken);

        return organizations
            .OrderBy(x => x.Name)
            .Select(Map)
            .ToList();
    }

    public async Task<OrganizationDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var organization =
            await _repository.GetByIdAsync(id, cancellationToken);

        return organization is null
            ? null
            : Map(organization);
    }

    public async Task<OrganizationDto> CreateAsync(
        CreateOrganizationRequest request,
        CancellationToken cancellationToken = default)
    {
        Validate(request.Name, request.Code);

        var code = request.Code.Trim();

        if (await _repository.ExistsByCodeAsync(
                code,
                null,
                cancellationToken))
        {
            throw new InvalidOperationException(
                "Organization code already exists.");
        }

        var organization = new NGD.OneVerse.Domain.Entities.Organization
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Code = code,
            Description = request.Description?.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(
            organization,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return Map(organization);
    }

    public async Task<bool> UpdateAsync(
        Guid id,
        UpdateOrganizationRequest request,
        CancellationToken cancellationToken = default)
    {
        Validate(request.Name, request.Code);

        var organization =
            await _repository.GetByIdAsync(
                id,
                cancellationToken);

        if (organization is null)
            return false;

        var code = request.Code.Trim();

        if (await _repository.ExistsByCodeAsync(
                code,
                id,
                cancellationToken))
        {
            throw new InvalidOperationException(
                "Organization code already exists.");
        }

        organization.Name = request.Name.Trim();
        organization.Code = code;
        organization.Description =
            request.Description?.Trim();
        organization.IsActive = request.IsActive;
        organization.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var organization =
            await _repository.GetByIdAsync(
                id,
                cancellationToken);

        if (organization is null)
            return false;

        _repository.Remove(organization);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    private static void Validate(
        string name,
        string code)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Organization name is required.",
                nameof(name));

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(
                "Organization code is required.",
                nameof(code));
    }

    private static OrganizationDto Map(NGD.OneVerse.Domain.Entities.Organization organization)
    {
        return new OrganizationDto(
            organization.Id,
            organization.Name,
            organization.Code,
            organization.Description,
            organization.IsActive,
            organization.CreatedAt,
            organization.UpdatedAt);
    }
}


