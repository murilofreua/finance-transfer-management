using Finance.Domain.Entities;

namespace Finance.Application.Interfaces.Repositories
{
    public interface ITransferAttemptRepository
    {
        Task AddAsync(TransferAttempt attempt);

        Task<List<TransferAttempt>> GetByAccountAndPeriodAsync(Guid accountId, DateTime startDate, DateTime endDate);
    }
}
