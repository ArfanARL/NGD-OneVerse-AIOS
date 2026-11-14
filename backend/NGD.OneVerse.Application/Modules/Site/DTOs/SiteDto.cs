namespace NGD.OneVerse.Application.Modules.Site.DTOs;

public sealed record SiteDto(
    Guid Id,
    Guid OrganizationId,
    string Name,
    string Code,
    string? Description,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
