using Finance.Application.Interfaces.Repositories;
using Finance.Domain.Entities;
using Finance.Domain.Enums;
using Finance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Finance.Infrastructure.Repositories
{
    public class TransferRepository : ITransferRepository
    {
        private readonly FinanceDbContext _context;

        public TransferRepository(FinanceDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Transfer transfer)
        {
            await _context.Transfers.AddAsync(transfer);
        }

        public async Task<Transfer?> GetByIdAsync(Guid id)
        {
            return await _context.Transfers
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Transfer>> GetScheduledToProcessAsync(DateTime currentDate)
        {
            return await _context.Transfers
                .Where(x =>
                    x.Status == TransferStatus.Scheduled &&
                    x.ScheduledAt <= currentDate)
                .ToListAsync();
        }

        public async Task<List<Transfer>> GetAllAsync()
        {
            return await _context.Transfers
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }
    }
}
