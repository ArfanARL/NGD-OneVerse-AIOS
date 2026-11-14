namespace NGD.OneVerse.Application.Modules.Organization.DTOs;

public sealed record CreateOrganizationRequest(
    string Name,
    string Code,
    string? Description
);
