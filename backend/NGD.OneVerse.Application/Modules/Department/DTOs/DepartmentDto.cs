namespace NGD.OneVerse.Application.Modules.Department.DTOs;

public sealed record DepartmentDto(
    Guid Id,
    Guid SiteId,
    string Name,
    string Code,
    string? Description,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
