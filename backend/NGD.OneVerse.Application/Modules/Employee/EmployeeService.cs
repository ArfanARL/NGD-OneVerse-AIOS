using NGD.OneVerse.Application.Modules.Employee.DTOs;
using NGD.OneVerse.Application.Modules.Employee.Interfaces;

namespace NGD.OneVerse.Application.Modules.Employee;

public sealed class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;

    public EmployeeService(IEmployeeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<EmployeeDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var employees =
            await _repository.GetAllAsync(cancellationToken);

        return employees
            .Select(Map)
            .ToList();
    }

    public async Task<EmployeeDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var employee =
            await _repository.GetByIdAsync(
                id,
                cancellationToken);

        return employee is null
            ? null
            : Map(employee);
    }

    public async Task<EmployeeDto> CreateAsync(
        CreateEmployeeRequest request,
        CancellationToken cancellationToken = default)
    {
        Validate(request);

        var employeeNumber = request.EmployeeNumber.Trim();
        var email = request.Email.Trim();

        if (await _repository.ExistsByEmployeeNumberAsync(
                employeeNumber,
                null,
                cancellationToken))
        {
            throw new InvalidOperationException(
                "Employee number already exists.");
        }

        if (await _repository.ExistsByEmailAsync(
                email,
                null,
                cancellationToken))
        {
            throw new InvalidOperationException(
                "Employee email already exists.");
        }

        if (!await _repository.OrganizationExistsAsync(
                request.OrganizationId,
                cancellationToken))
        {
            throw new ArgumentException(
                "Organization does not exist or is inactive.",
                nameof(request.OrganizationId));
        }

        if (!await _repository.DepartmentExistsAsync(
                request.DepartmentId,
                cancellationToken))
        {
            throw new ArgumentException(
                "Department does not exist or is inactive.",
                nameof(request.DepartmentId));
        }

        if (request.SiteId.HasValue)
        {
            if (!await _repository.SiteExistsAsync(
                    request.SiteId.Value,
                    cancellationToken))
            {
                throw new ArgumentException(
                    "Site does not exist or is inactive.",
                    nameof(request.SiteId));
            }
        }

        var employee =
            new NGD.OneVerse.Domain.Entities.Employee
            {
                Id = Guid.NewGuid(),
                EmployeeNumber = employeeNumber,
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = email,
                PhoneNumber = request.PhoneNumber?.Trim(),
                DateOfBirth = request.DateOfBirth,
                DateOfJoining = request.DateOfJoining,
                DepartmentId = request.DepartmentId,
                OrganizationId = request.OrganizationId,
                SiteId = request.SiteId,
                JobTitle = request.JobTitle?.Trim(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

        await _repository.AddAsync(
            employee,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return Map(employee);
    }

    public async Task<bool> UpdateAsync(
        Guid id,
        UpdateEmployeeRequest request,
        CancellationToken cancellationToken = default)
    {
        Validate(request);

        var employee =
            await _repository.GetByIdAsync(
                id,
                cancellationToken);

        if (employee is null)
            return false;

        var employeeNumber = request.EmployeeNumber.Trim();
        var email = request.Email.Trim();

        if (await _repository.ExistsByEmployeeNumberAsync(
                employeeNumber,
                id,
                cancellationToken))
        {
            throw new InvalidOperationException(
                "Employee number already exists.");
        }

        if (await _repository.ExistsByEmailAsync(
                email,
                id,
                cancellationToken))
        {
            throw new InvalidOperationException(
                "Employee email already exists.");
        }

        if (!await _repository.OrganizationExistsAsync(
                request.OrganizationId,
                cancellationToken))
        {
            throw new ArgumentException(
                "Organization does not exist or is inactive.",
                nameof(request.OrganizationId));
        }

        if (!await _repository.DepartmentExistsAsync(
                request.DepartmentId,
                cancellationToken))
        {
            throw new ArgumentException(
                "Department does not exist or is inactive.",
                nameof(request.DepartmentId));
        }

        if (request.SiteId.HasValue)
        {
            if (!await _repository.SiteExistsAsync(
                    request.SiteId.Value,
                    cancellationToken))
            {
                throw new ArgumentException(
                    "Site does not exist or is inactive.",
                    nameof(request.SiteId));
            }
        }

        employee.EmployeeNumber = employeeNumber;
        employee.FirstName = request.FirstName.Trim();
        employee.LastName = request.LastName.Trim();
        employee.Email = email;
        employee.PhoneNumber = request.PhoneNumber?.Trim();
        employee.DateOfBirth = request.DateOfBirth;
        employee.DateOfJoining = request.DateOfJoining;
        employee.DepartmentId = request.DepartmentId;
        employee.OrganizationId = request.OrganizationId;
        employee.SiteId = request.SiteId;
        employee.JobTitle = request.JobTitle?.Trim();
        employee.IsActive = request.IsActive;
        employee.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var employee =
            await _repository.GetByIdAsync(
                id,
                cancellationToken);

        if (employee is null)
            return false;

        employee.IsActive = false;
        employee.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    private static void Validate(CreateEmployeeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.EmployeeNumber))
            throw new ArgumentException(
                "Employee number is required.",
                nameof(request.EmployeeNumber));

        if (string.IsNullOrWhiteSpace(request.FirstName))
            throw new ArgumentException(
                "First name is required.",
                nameof(request.FirstName));

        if (string.IsNullOrWhiteSpace(request.LastName))
            throw new ArgumentException(
                "Last name is required.",
                nameof(request.LastName));

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException(
                "Email is required.",
                nameof(request.Email));

        if (request.OrganizationId == Guid.Empty)
            throw new ArgumentException(
                "Organization is required.",
                nameof(request.OrganizationId));

        if (request.DepartmentId == Guid.Empty)
            throw new ArgumentException(
                "Department is required.",
                nameof(request.DepartmentId));
    }

    private static void Validate(UpdateEmployeeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.EmployeeNumber))
            throw new ArgumentException(
                "Employee number is required.",
                nameof(request.EmployeeNumber));

        if (string.IsNullOrWhiteSpace(request.FirstName))
            throw new ArgumentException(
                "First name is required.",
                nameof(request.FirstName));

        if (string.IsNullOrWhiteSpace(request.LastName))
            throw new ArgumentException(
                "Last name is required.",
                nameof(request.LastName));

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException(
                "Email is required.",
                nameof(request.Email));

        if (request.OrganizationId == Guid.Empty)
            throw new ArgumentException(
                "Organization is required.",
                nameof(request.OrganizationId));

        if (request.DepartmentId == Guid.Empty)
            throw new ArgumentException(
                "Department is required.",
                nameof(request.DepartmentId));
    }

    private static EmployeeDto Map(
        NGD.OneVerse.Domain.Entities.Employee employee)
    {
        return new EmployeeDto(
            employee.Id,
            employee.EmployeeNumber,
            employee.FirstName,
            employee.LastName,
            employee.Email,
            employee.PhoneNumber,
            employee.DateOfBirth,
            employee.DateOfJoining,
            employee.DepartmentId,
            employee.OrganizationId,
            employee.SiteId,
            employee.JobTitle,
            employee.IsActive,
            employee.CreatedAt,
            employee.UpdatedAt);
    }
}
