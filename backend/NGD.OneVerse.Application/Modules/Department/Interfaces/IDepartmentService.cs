using NGD.OneVerse.Application.Modules.Department.DTOs;

namespace NGD.OneVerse.Application.Modules.Department.Interfaces;

public interface IDepartmentService
{
    Task<IReadOnlyList<DepartmentDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<DepartmentDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<DepartmentDto> CreateAsync(
        CreateDepartmentRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        Guid id,
        UpdateDepartmentRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}