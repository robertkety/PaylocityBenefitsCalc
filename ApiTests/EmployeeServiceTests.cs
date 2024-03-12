using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Models;
using Api.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ApiTests
{
    public class EmployeeServiceTests : IDisposable
    {
        private DatabaseContext _dbContext;
        private EmployeeService _employeeService;

        public EmployeeServiceTests()
        {
            _dbContext = new DatabaseContext(GetInMemoryOptions());
            _dbContext.Database.EnsureCreated();
            _dbContext.Employees.AddRange(GetEmployees().Select(x => x.ToEmployee()));
            _dbContext.SaveChanges();
            _employeeService = new EmployeeService(_dbContext);
        }

        [Fact]
        public void AddEmployeesAsync_Test()
        {
            var oldEmployees = _dbContext.Employees.ToList();
            var newEmployees = new[]
            {
                new AddEmployeeDto
                {
                    DateOfBirth = DateTime.UtcNow,
                    FirstName = Guid.NewGuid().ToString(),
                    LastName = Guid.NewGuid().ToString()
                },
                new AddEmployeeDto
                {
                    DateOfBirth = DateTime.UtcNow,
                    FirstName = Guid.NewGuid().ToString(),
                    LastName = Guid.NewGuid().ToString()
                },
                new AddEmployeeDto
                {
                    DateOfBirth = DateTime.UtcNow,
                    FirstName = Guid.NewGuid().ToString(),
                    LastName = Guid.NewGuid().ToString()
                }
            };
            var addedEmployees = _employeeService.AddEmployeesAsync(newEmployees, default).GetAwaiter().GetResult();

            Assert.Equal(newEmployees.Count(), addedEmployees.Count());
            Assert.Equal(oldEmployees.Count() + addedEmployees.Count(), _dbContext.Employees.Count());
        }

        [Fact]
        public void GetAllEmployeesAsync_Test()
        {
            var expectedEmployees = _dbContext.Employees.ToList();
            var actualEmployees = _employeeService.GetAllEmployeesAsync(default).GetAwaiter().GetResult();
            Assert.Equal(expectedEmployees.Count(), actualEmployees.Count());
        }

        [Fact]
        public void GetEmployeeByIdAsync_Test()
        {
            var employeeId = 1;
            var result = _employeeService.GetEmployeeByIdAsync(employeeId, default).GetAwaiter().GetResult();
            Assert.NotNull(result);
        }

        [Fact]
        public void GetEmployeeByIdAsync_EmployeeIdNotFound_Test() =>
            Assert.Throws<EmployeeIdNotFoundException>(() => _employeeService.GetEmployeeByIdAsync(3000, default).GetAwaiter().GetResult());

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
        }

        internal static IEnumerable<AddEmployeeDto> GetEmployees() => new List<AddEmployeeDto>
        {
            new()
            {
                FirstName = "LeBron",
                LastName = "James",
                Salary = 75420.99m,
                DateOfBirth = new DateTime(1984, 12, 30)
            },
            new()
            {
                FirstName = "Ja",
                LastName = "Morant",
                Salary = 92365.22m,
                DateOfBirth = new DateTime(1999, 8, 10),
                Dependents = new List<AddDependentDto>
                {
                    new()
                    {
                        FirstName = "Spouse",
                        LastName = "Morant",
                        Relationship = Relationship.Spouse,
                        DateOfBirth = new DateTime(1998, 3, 3)
                    },
                    new()
                    {
                        FirstName = "Child1",
                        LastName = "Morant",
                        Relationship = Relationship.Child,
                        DateOfBirth = new DateTime(2020, 6, 23)
                    },
                    new()
                    {
                        FirstName = "Child2",
                        LastName = "Morant",
                        Relationship = Relationship.Child,
                        DateOfBirth = new DateTime(2021, 5, 18)
                    }
                }
            },
            new()
            {
                FirstName = "Michael",
                LastName = "Jordan",
                Salary = 143211.12m,
                DateOfBirth = new DateTime(1963, 2, 17),
                Dependents = new List<AddDependentDto>
                {
                    new()
                    {
                        FirstName = "DP",
                        LastName = "Jordan",
                        Relationship = Relationship.DomesticPartner,
                        DateOfBirth = new DateTime(1974, 1, 2)
                    }
                }
            }
        };

        internal static DbContextOptions<DatabaseContext> GetInMemoryOptions() => new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
    }
}
