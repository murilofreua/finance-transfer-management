using Finance.Application.Interfaces.Repositories;
using Finance.Domain.Entities;
using Finance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Finance.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly FinanceDbContext _context;

        public AccountRepository(FinanceDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Account account)
        {
            await _context.Accounts.AddAsync(account);
        }

        public async Task<Account?> GetByIdAsync(Guid id)
        {
            return await _context.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Account?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Account>> GetAllAsync()
        {
            return await _context.Accounts
                .AsNoTracking()
                .OrderBy(a => a.PersonName)
                .ToListAsync();
        }
    }
}
