namespace NGD.OneVerse.Application.Modules.Employee.Interfaces;

using NGD.OneVerse.Application.Modules.Employee.DTOs;

public interface IEmployeeService
{
    Task<IReadOnlyList<EmployeeDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<EmployeeDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<EmployeeDto> CreateAsync(
        CreateEmployeeRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        Guid id,
        UpdateEmployeeRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
