using Finance.Application.Interfaces.Repositories;
using Finance.Application.Interfaces.UnitOfWork;

namespace Finance.Application.UseCases.Transfers.CancelTransfer
{
    public class CancelTransferHandler
    {
        private readonly ITransferRepository _transferRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CancelTransferHandler(
            ITransferRepository transferRepository,
            IUnitOfWork unitOfWork)
        {
            _transferRepository = transferRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task HandleAsync(Guid transferId)
        {
            var transfer = await _transferRepository.GetByIdAsync(transferId);

            if (transfer is null)
            {
                throw new InvalidOperationException("Transferência não encontrada.");
            }

            transfer.Cancel();

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
