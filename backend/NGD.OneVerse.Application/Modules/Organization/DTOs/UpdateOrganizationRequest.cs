namespace NGD.OneVerse.Application.Modules.Organization.DTOs;

public sealed record UpdateOrganizationRequest(
    string Name,
    string Code,
    string? Description,
    bool IsActive
);
