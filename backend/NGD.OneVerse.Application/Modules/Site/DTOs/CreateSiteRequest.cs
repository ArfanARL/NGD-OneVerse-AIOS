namespace NGD.OneVerse.Application.Modules.Site.DTOs;

public sealed record CreateSiteRequest(
    Guid OrganizationId,
    string Name,
    string Code,
    string? Description);
