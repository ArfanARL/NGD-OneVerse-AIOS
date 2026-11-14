using System;

namespace NGD.OneVerse.Domain.Entities;

public class Employee
{
    public Guid Id { get; set; }

    public string EmployeeNumber { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public DateTime DateOfJoining { get; set; }

    public Guid DepartmentId { get; set; }

    public Guid OrganizationId { get; set; }

    public Guid? SiteId { get; set; }

    public string? JobTitle { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties (optional)
    public Organization? Organization { get; set; }
    public Department? Department { get; set; }
    public Site? Site { get; set; }
}
