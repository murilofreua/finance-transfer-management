using Finance.Application.Interfaces.Repositories;
using Finance.Domain.Entities;
using Finance.Domain.Enums;
using Finance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Finance.Infrastructure.Repositories
{
    public class TransferLimitRepository : ITransferLimitRepository
    {
        private readonly FinanceDbContext _context;

        public TransferLimitRepository(FinanceDbContext context)
        {
            _context = context;
        }

        public async Task<TransferLimit?> GetByPeriodAsync(TransferPeriod period)
        {
            return await _context.TransferLimits
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Period == period);
        }
    }
}
