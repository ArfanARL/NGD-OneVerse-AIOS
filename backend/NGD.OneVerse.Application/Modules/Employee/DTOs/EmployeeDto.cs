namespace NGD.OneVerse.Application.Modules.Employee.DTOs;

public sealed record EmployeeDto(
    Guid Id,
    string EmployeeNumber,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    DateTime? DateOfBirth,
    DateTime DateOfJoining,
    Guid DepartmentId,
    Guid OrganizationId,
    Guid? SiteId,
    string? JobTitle,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
