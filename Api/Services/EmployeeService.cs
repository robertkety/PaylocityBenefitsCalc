using Api.Dtos.Employee;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly DatabaseContext _databaseContext;

        public EmployeeService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<IEnumerable<GetEmployeeDto>> AddEmployeesAsync(IEnumerable<AddEmployeeDto> employees, CancellationToken cancellationToken)
        {
            var newEmployees = employees.Select(e => e.ToEmployee());
            await _databaseContext.Employees.AddRangeAsync(newEmployees);
            await _databaseContext.SaveChangesAsync(cancellationToken);
            return newEmployees.Select(e => new GetEmployeeDto(e));
        }

        public async Task<IEnumerable<GetEmployeeDto>> GetAllEmployeesAsync(CancellationToken cancellationToken)
        {
            return await _databaseContext.Employees.Include(e => e.Dependents).Select(e => new GetEmployeeDto(e)).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets an employee by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="EmployeeIdNotFoundException"></exception>
        public async Task<GetEmployeeDto> GetEmployeeByIdAsync(int id, CancellationToken cancellationToken)
        {
            if (!await _databaseContext.Employees.ContainsAsync(new Employee { Id = id }))
                throw new EmployeeIdNotFoundException(id, $"Employee Id {id} Not Found");
            return new GetEmployeeDto(await _databaseContext.Employees.Include(e => e.Dependents).SingleAsync(e => e.Id == id, cancellationToken));
        }
    }
}
