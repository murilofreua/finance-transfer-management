using Finance.Application.Interfaces.Repositories;
using Finance.Domain.Entities;
using Finance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Finance.Infrastructure.Repositories
{
    public class TransferAttemptRepository : ITransferAttemptRepository
    {
        private readonly FinanceDbContext _context;

        public TransferAttemptRepository(FinanceDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TransferAttempt attempt)
        {
            await _context.TransferAttempts.AddAsync(attempt);
        }

        public async Task<List<TransferAttempt>> GetByAccountAndPeriodAsync(Guid accountId, DateTime startDate, DateTime endDate)
        {
            return await _context.TransferAttempts
                .AsNoTracking()
                .Where(x =>
                    x.AccountId == accountId &&
                    x.CreatedAt >= startDate &&
                    x.CreatedAt < endDate)
                .ToListAsync();
        }
    }
}
