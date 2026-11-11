using Finance.Domain.Entities;

namespace Finance.Application.Interfaces.Repositories
{
    public interface ITransferRepository
    {
        Task AddAsync(Transfer transfer);

        Task<Transfer?> GetByIdAsync(Guid id);

        Task<List<Transfer>> GetScheduledToProcessAsync(DateTime currentDate);

        Task<List<Transfer>> GetAllAsync();
    }
}
