using Api.Dtos.Employee;

namespace Api.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<GetEmployeeDto>> AddEmployeesAsync(IEnumerable<AddEmployeeDto> employees, CancellationToken cancellationToken);
        Task<IEnumerable<GetEmployeeDto>> GetAllEmployeesAsync(CancellationToken cancellationToken);
        Task<GetEmployeeDto> GetEmployeeByIdAsync(int id, CancellationToken cancellationToken);
    }
}