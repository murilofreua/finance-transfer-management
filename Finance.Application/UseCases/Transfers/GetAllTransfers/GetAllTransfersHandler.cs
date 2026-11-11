using Finance.Application.DTOs.Transfers;
using Finance.Application.Interfaces.Repositories;

namespace Finance.Application.UseCases.Transfers.GetAllTransfers
{
    public class GetAllTransfersHandler
    {
        private readonly ITransferRepository _transferRepository;

        public GetAllTransfersHandler(ITransferRepository transferRepository)
        {
            _transferRepository = transferRepository;
        }

        public async Task<List<TransferResponse>> HandleAsync()
        {
            var transfers = await _transferRepository.GetAllAsync();

            return transfers.Select(transfer => new TransferResponse
            {
                Id = transfer.Id,
                SourceAccountId = transfer.SourceAccountId,
                DestinationAccountId = transfer.DestinationAccountId,
                Amount = transfer.Amount,
                Status = transfer.Status.ToString(),
                CreatedAt = transfer.CreatedAt,
                ScheduledAt = transfer.ScheduledAt.HasValue ? DateTime.SpecifyKind(transfer.ScheduledAt.Value, DateTimeKind.Utc) : null
            }).ToList();
        }
    }
}
