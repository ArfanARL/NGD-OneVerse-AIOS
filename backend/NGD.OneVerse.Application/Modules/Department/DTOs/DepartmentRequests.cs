namespace NGD.OneVerse.Application.Modules.Department.DTOs;

public sealed record CreateDepartmentRequest(
    Guid SiteId,
    string Name,
    string Code,
    string? Description);

public sealed record UpdateDepartmentRequest(
    Guid SiteId,
    string Name,
    string Code,
    string? Description,
    bool IsActive);
