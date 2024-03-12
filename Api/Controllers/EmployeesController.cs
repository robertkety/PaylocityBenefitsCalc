using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly EmployeeService _employeeService;

    public EmployeesController(EmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [SwaggerOperation(Summary = "Get employee by id")]
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<GetEmployeeDto>>> Get(int id, CancellationToken cancellationToken)
    {
        var result = new ApiResponse<GetEmployeeDto>
        {
            Success = true,
            Data = null
        };
        try
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id, cancellationToken);
            result.Data = employee;
        }
        catch (EmployeeIdNotFoundException ex)
        {
            result.Success = false;
            result.Error = nameof(EmployeeIdNotFoundException);
            result.Message = ex.Message;

            return NotFound(result);
        }

        return Ok(result);
    }

    [SwaggerOperation(Summary = "Get all employees")]
    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<GetEmployeeDto>>>> GetAll(CancellationToken cancellationToken)
    {
        var employees = await _employeeService.GetAllEmployeesAsync(cancellationToken);
        var result = new ApiResponse<List<GetEmployeeDto>>
        {
            Data = employees.ToList(),
            Success = true
        };

        return result;
    }

    // todo: I prioritized a batch endpoint to get the database populated. Depending on api stakeholders, a more restful approach might be warranted
    [SwaggerOperation(Summary = "Adds employees and their dependents")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<GetEmployeeDto>>>> AddEmployees(IEnumerable<AddEmployeeDto> employees, CancellationToken cancellationToken)
    {
        var newEmployees = await _employeeService.AddEmployeesAsync(employees, cancellationToken);
        var result = new ApiResponse<List<GetEmployeeDto>>
        {
            Data = newEmployees.ToList(),
            Success = true
        };

        return result;
    }

    [SwaggerOperation(Summary = "Cost")]
    [HttpGet("cost")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<GetPaycheckDetailDto>>> GetPaycheckDetails(int employeeId, CancellationToken cancellationToken)
    {
        var result = new ApiResponse<GetPaycheckDetailDto>
        {
            Success = true,
            Data = null
        };

        // get employee
        GetEmployeeDto? employee;
        try
        {
            employee = await _employeeService.GetEmployeeByIdAsync(employeeId, cancellationToken);
        }
        catch (EmployeeIdNotFoundException ex)
        {
            result.Success = false;
            result.Error = nameof(EmployeeIdNotFoundException);
            result.Message = ex.Message;

            return NotFound(result);
        }

        result.Data = new GetPaycheckDetailDto();
        result.Data.GrossCompensation = PaycheckService.CalculateGrossCompensationPerPayPeriod(employee.Salary);
        result.Data.Deductions = PaycheckService.CalculateDeductionPerPayPersion(employee.Salary, employee.Dependents.Count(), employee.Dependents.Where(d => PaycheckService.CalculateAge(d.DateOfBirth) > 50).Count());

        return Ok(result);
    }
}
