using Api.Dtos.Dependent;

namespace Api.Services
{
    public interface IDependentService
    {
        public Task<IEnumerable<GetDependentDto>> AddDependentsAsync(IEnumerable<AddDependentDto> dependents, CancellationToken cancellationToken);
        public Task<IEnumerable<GetDependentDto>> GetAllDependentsAsync(CancellationToken cancellationToken);
        public Task<GetDependentDto> GetDependentByIdAsync(int id, CancellationToken cancellationToken);
    }
}