using Finance.Domain.Entities;
using Finance.Domain.Enums;

namespace Finance.Application.Interfaces.Repositories
{
    public interface ITransferLimitRepository
    {
        Task<TransferLimit?> GetByPeriodAsync(TransferPeriod period);
    }
}
