using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Models;
using Api.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ApiTests
{
    public class DependentServiceTests : IDisposable
    {
        private DatabaseContext _dbContext;
        private DependentService _dependentService;

        public DependentServiceTests()
        {
            _dbContext = new DatabaseContext(EmployeeServiceTests.GetInMemoryOptions());
            _dbContext.Database.EnsureCreated();
            _dependentService = new DependentService(_dbContext);
            new EmployeeService(_dbContext).AddEmployeesAsync(EmployeeServiceTests.GetEmployees(), default).GetAwaiter().GetResult();
        }

        [Fact]
        public void AddDependentsAsync_Test()
        {
            var employeeId = 3;
            var oldDependents = _dbContext.Dependents.Where(x => x.EmployeeId == employeeId).ToList();
            var newDependents = new[]
            {
                new AddDependentDto
                {
                    DateOfBirth = DateTime.UtcNow,
                    EmployeeId = employeeId,
                    FirstName = Guid.NewGuid().ToString(),
                    LastName = Guid.NewGuid().ToString(),
                    Relationship = Relationship.Child
                },
                new AddDependentDto
                {
                    DateOfBirth = DateTime.UtcNow,
                    EmployeeId = employeeId,
                    FirstName = Guid.NewGuid().ToString(),
                    LastName = Guid.NewGuid().ToString(),
                    Relationship = Relationship.Child
                },
                new AddDependentDto
                {
                    DateOfBirth = DateTime.UtcNow,
                    EmployeeId = employeeId,
                    FirstName = Guid.NewGuid().ToString(),
                    LastName = Guid.NewGuid().ToString(),
                    Relationship = Relationship.Child
                }
            };
            var addedDependents = _dependentService.AddDependentsAsync(newDependents, default).GetAwaiter().GetResult();

            Assert.Equal(newDependents.Count(), addedDependents.Count());
            Assert.Equal(oldDependents.Count() + addedDependents.Count(), _dbContext.Dependents.Where(x => x.EmployeeId == employeeId).Count());
        }

        [Fact]
        public void AddDependentsAsync_EmployeeIdNotFound_Test() =>
            Assert.Throws<EmployeeIdNotFoundException>(() => _dependentService.AddDependentsAsync(new[]
            {
                new AddDependentDto
                {
                    DateOfBirth = DateTime.UtcNow,
                    EmployeeId = 3000,
                    FirstName = Guid.NewGuid().ToString(),
                    LastName = Guid.NewGuid().ToString(),
                    Relationship = Relationship.Spouse
                }
            }, default).GetAwaiter().GetResult());

        [Fact]
        public void AddDependentsAsync_EnforceSinglePartner_DomesticPartnerAndSpouse_Test() =>
            Assert.Throws<SinglePartnerDependencyException>(() => _dependentService.AddDependentsAsync(new[]
            {
                new AddDependentDto
                {
                    DateOfBirth = DateTime.UtcNow,
                    EmployeeId = 3,
                    FirstName = Guid.NewGuid().ToString(),
                    LastName = Guid.NewGuid().ToString(),
                    Relationship = Relationship.Spouse
                }
            }, default).GetAwaiter().GetResult());

        [Fact]
        public void AddDependentsAsync_EnforceSinglePartner_SpouseAndDomesticPartner_Test() =>
            Assert.Throws<SinglePartnerDependencyException>(() => _dependentService.AddDependentsAsync(new[]
            {
                new AddDependentDto
                {
                    DateOfBirth = DateTime.UtcNow,
                    EmployeeId = 2,
                    FirstName = Guid.NewGuid().ToString(),
                    LastName = Guid.NewGuid().ToString(),
                    Relationship = Relationship.DomesticPartner
                }
            }, default).GetAwaiter().GetResult());

        [Fact]
        public void AddDependentsAsync_EnforceSinglePartner_SpouseAndSpouse_Test() =>
            Assert.Throws<SinglePartnerDependencyException>(() => _dependentService.AddDependentsAsync(new[]
            {
                new AddDependentDto
                {
                    DateOfBirth = DateTime.UtcNow,
                    EmployeeId = 2,
                    FirstName = Guid.NewGuid().ToString(),
                    LastName = Guid.NewGuid().ToString(),
                    Relationship = Relationship.Spouse
                }
            }, default).GetAwaiter().GetResult());

        [Fact]
        public void GetAllDependentsAsync()
        {
            var expectedDependents = _dependentService.GetAllDependentsAsync(default).GetAwaiter().GetResult();
            var actualDependents = _dbContext.Dependents.ToList();

            Assert.Equal(expectedDependents.Count(), actualDependents.Count());
        }

        [Fact]
        public void GetDependentByIdAsync_Test()
        {
            var dependentId = 1;
            var result = _dependentService.GetDependentByIdAsync(dependentId, default);
            Assert.NotNull(result);
        }

        [Fact]
        public void GetDependentByIdAsync_DependentIdNotFound_Test() =>
            Assert.Throws<DependentIdNotFoundException>(() => _dependentService.GetDependentByIdAsync(3000, default).GetAwaiter().GetResult());

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
        }
    }
}
