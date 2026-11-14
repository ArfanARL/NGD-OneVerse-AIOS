using NGD.OneVerse.Application.Modules.Department.DTOs;
using NGD.OneVerse.Application.Modules.Department.Interfaces;
using NGD.OneVerse.Domain.Entities;

using DepartmentEntity = global::NGD.OneVerse.Domain.Entities.Department;
namespace NGD.OneVerse.Application.Modules.Department;

public sealed class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _repository;

    public DepartmentService(IDepartmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<DepartmentDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var departments = await _repository.GetAllAsync(cancellationToken);
        return departments.OrderBy(x => x.Name).Select(Map).ToList();
    }

    public async Task<DepartmentDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var department = await _repository.GetByIdAsync(id, cancellationToken);
        return department is null ? null : Map(department);
    }

    public async Task<DepartmentDto> CreateAsync(
        CreateDepartmentRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        Validate(request.SiteId, request.Name, request.Code);

        var code = request.Code.Trim();
        if (!await _repository.SiteExistsAsync(request.SiteId, cancellationToken))
        {
            throw new InvalidOperationException("Site does not exist or is inactive.");
        }

        if (await _repository.ExistsByCodeAsync(request.SiteId, code, null, cancellationToken))
        {
            throw new InvalidOperationException("Department code already exists for this site.");
        }

        var department = new Department
        {
            Id = Guid.NewGuid(),
            SiteId = request.SiteId,
            Name = request.Name.Trim(),
            Code = code,
            Description = request.Description?.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(department, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return Map(department);
    }

    public async Task<bool> UpdateAsync(
        Guid id,
        UpdateDepartmentRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        Validate(request.SiteId, request.Name, request.Code);

        var department = await _repository.GetByIdAsync(id, cancellationToken);
        if (department is null)
        {
            return false;
        }

        var code = request.Code.Trim();
        if (!await _repository.SiteExistsAsync(request.SiteId, cancellationToken))
        {
            throw new InvalidOperationException("Site does not exist or is inactive.");
        }

        if (await _repository.ExistsByCodeAsync(request.SiteId, code, id, cancellationToken))
        {
            throw new InvalidOperationException("Department code already exists for this site.");
        }

        department.SiteId = request.SiteId;
        department.Name = request.Name.Trim();
        department.Code = code;
        department.Description = request.Description?.Trim();
        department.IsActive = request.IsActive;
        department.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var department = await _repository.GetByIdAsync(id, cancellationToken);
        if (department is null)
        {
            return false;
        }

        _repository.Remove(department);
        await _repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static void Validate(Guid siteId, string name, string code)
    {
        if (siteId == Guid.Empty)
        {
            throw new ArgumentException("Site is required.", nameof(siteId));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Department name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Department code is required.", nameof(code));
        }
    }

    private static DepartmentDto Map(global::NGD.OneVerse.Domain.Entities.Department department)
    {
        return new DepartmentDto(
            department.Id,
            department.SiteId,
            department.Name,
            department.Code,
            department.Description,
            department.IsActive,
            department.CreatedAt,
            department.UpdatedAt);
    }
}