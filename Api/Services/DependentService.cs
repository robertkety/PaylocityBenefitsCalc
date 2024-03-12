using Api.Dtos.Dependent;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class DependentService : IDependentService
    {
        private readonly DatabaseContext _databaseContext;

        public DependentService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<IEnumerable<GetDependentDto>> AddDependentsAsync(IEnumerable<AddDependentDto> dependents, CancellationToken cancellationToken)
        {
            try
            {
                // Handle unknown employee ids
                var employeeIds = dependents.Select(d => d.EmployeeId).Distinct();
                var unknownIds = employeeIds.Except(_databaseContext.Employees.Where(e => employeeIds.Contains(e.Id)).Select(e => e.Id));
                if (unknownIds.Any())
                    throw new EmployeeIdNotFoundException(unknownIds.First(), $"Employee Ids Not Found: {string.Join(", ", unknownIds)}"); // Good use case for serialization here, but keeping it simple
                
                // Enforce single partner dependent per employee requirement
                var newPartnersByEmployeeId = dependents
                    .Where(d => d.Relationship == Relationship.Spouse || d.Relationship == Relationship.DomesticPartner)
                    .Select(d => d.EmployeeId)
                    .Distinct();
                var partnerRelationship = new[] { Relationship.Spouse, Relationship.DomesticPartner }.ToList();
                var employeesWithAPartnerDependencyConflict = _databaseContext.Dependents.Where(d => partnerRelationship.Contains(d.Relationship) && newPartnersByEmployeeId.Contains(d.EmployeeId));
                if (employeesWithAPartnerDependencyConflict.Any())
                    // todo: this might be better with id support (assuming we don't want security through obscurity)
                    throw new SinglePartnerDependencyException("An employee may not have more than one Domestic Partner or Spouse as a dependent");

                var newDependents = dependents.Select(e => e.ToDependent());
                await _databaseContext.Dependents.AddRangeAsync(newDependents);
                await _databaseContext.SaveChangesAsync(cancellationToken);
                return newDependents.Select(e => new GetDependentDto(e));
            }
            // This saves me a trip to the database looking for an existing partner for each employee in the dto batch, but the
            // complexity of handling it demonstrates an example of why we may want to handle business logic at the service layer
            catch (DbUpdateException ex) when (ex?.InnerException?.Message?.Contains("IX_Dependents_EmployeeId_IsPartner") ?? false)
            {
                // todo: this might be better with id support (assuming we don't want security through obscurity)
                throw new SinglePartnerDependencyException("An employee may not have more than one Domestic Partner or Spouse as a dependent");
            }
        }

        public async Task<IEnumerable<GetDependentDto>> GetAllDependentsAsync(CancellationToken cancellationToken)
        {
            return await _databaseContext.Dependents.Select(e => new GetDependentDto(e)).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets an Dependent by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="DependentIdNotFoundException"></exception>
        public async Task<GetDependentDto> GetDependentByIdAsync(int id, CancellationToken cancellationToken)
        {
            if (!await _databaseContext.Dependents.ContainsAsync(new Dependent { Id = id }))
                throw new DependentIdNotFoundException(id, $"Dependent Id {id} Not Found");
            return new GetDependentDto(await _databaseContext.Dependents.SingleAsync(e => e.Id == id, cancellationToken));
        }
    }
}
