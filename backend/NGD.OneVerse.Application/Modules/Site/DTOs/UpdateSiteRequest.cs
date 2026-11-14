namespace NGD.OneVerse.Application.Modules.Site.DTOs;

public sealed record UpdateSiteRequest(
    Guid OrganizationId,
    string Name,
    string Code,
    string? Description,
    bool IsActive);
