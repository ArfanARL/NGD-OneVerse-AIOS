namespace NGD.OneVerse.Application.Modules.Organization.DTOs;

public sealed record OrganizationDto(
    Guid Id,
    string Name,
    string Code,
    string? Description,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
