using Finance.Domain.Entities;

namespace Finance.Application.Interfaces.Repositories
{
    public interface IAccountRepository
    {
        Task AddAsync(Account account);

        Task<Account?> GetByIdAsync(Guid id);

        Task<Account?> GetByIdForUpdateAsync(Guid id);

        Task<List<Account>> GetAllAsync();
    }
}
