using Api.Models;

namespace Api.Dtos.Dependent;

public class AddDependentDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    // todo: enum sanitization would be good here
    public Relationship Relationship { get; set; }
    public int EmployeeId { get; set; }

    public Models.Dependent ToDependent()
    {
        return new Models.Dependent
        {
            FirstName = FirstName,
            LastName = LastName,
            DateOfBirth = DateOfBirth,
            Relationship = Relationship,
            EmployeeId = EmployeeId
        };
    }
}
