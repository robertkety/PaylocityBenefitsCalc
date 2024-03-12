using Api.Dtos.Dependent;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class DependentsController : ControllerBase
{
    private readonly DependentService _dependentService;

    public DependentsController(DependentService dependentService)
    {
        _dependentService = dependentService;
    }

    [SwaggerOperation(Summary = "Get dependent by id")]
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<GetDependentDto>>> Get(int id, CancellationToken cancellationToken)
    {
        var result = new ApiResponse<GetDependentDto>
        {
            Success = true,
            Data = null
        };
        try
        {
            var dependent = await _dependentService.GetDependentByIdAsync(id, cancellationToken);
            result.Data = dependent;
        }
        catch (DependentIdNotFoundException ex)
        {
            result.Success = false;
            result.Error = nameof(DependentIdNotFoundException);
            result.Message = ex.Message;

            return NotFound(result);
        }

        return Ok(result);
    }

    [SwaggerOperation(Summary = "Get all dependents")]
    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<GetDependentDto>>>> GetAll(CancellationToken cancellationToken)
    {
        var dependents = await _dependentService.GetAllDependentsAsync(cancellationToken);
        var result = new ApiResponse<List<GetDependentDto>>
        {
            Data = dependents.ToList(),
            Success = true
        };

        return result;
    }

    // todo: I prioritized a batch endpoint to get the database populated. Depending on api stakeholders, a more restful approach might be warranted
    [SwaggerOperation(Summary = "Adds dependents to existing employees")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<GetDependentDto>>>> AddDependents(IEnumerable<AddDependentDto> dependents, CancellationToken cancellationToken)
    {
        var result = new ApiResponse<List<GetDependentDto>>
        {
            Data = null,
            Success = true
        };

        try
        {
            result.Data = (await _dependentService.AddDependentsAsync(dependents, cancellationToken)).ToList();
        }
        catch (EmployeeIdNotFoundException ex)
        {
            result.Success = false;
            result.Error = nameof(EmployeeIdNotFoundException);
            result.Message = ex.Message;

            return NotFound(result);
        }
        catch (SinglePartnerDependencyException ex)
        {
            result.Success = false;
            result.Error = nameof(SinglePartnerDependencyException);
            result.Message = ex.Message;

            return NotFound(result);
        }
        

        return result;
    }
}
