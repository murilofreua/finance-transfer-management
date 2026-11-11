using Finance.Application.Interfaces.Repositories;
using Finance.Application.Interfaces.UnitOfWork;
using Finance.Application.Services;
using Finance.Domain.Entities;
using Finance.Domain.Enums;

namespace Finance.Application.UseCases.Transfers.ProcessScheduledTransfers
{
    public class ProcessScheduledTransfersHandler
    {
        private readonly ITransferRepository _transferRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ITransferAttemptRepository _attemptRepository;
        private readonly TransferLimitService _limitService;
        private readonly IUnitOfWork _unitOfWork;

        public ProcessScheduledTransfersHandler(
            ITransferRepository transferRepository,
            IAccountRepository accountRepository,
            ITransferAttemptRepository attemptRepository,
            TransferLimitService limitService,
            IUnitOfWork unitOfWork)
        {
            _transferRepository = transferRepository;
            _accountRepository = accountRepository;
            _attemptRepository = attemptRepository;
            _limitService = limitService;
            _unitOfWork = unitOfWork;
        }

        public async Task HandleAsync()
        {
            var currentDate = DateTime.UtcNow;

            var transfers = await _transferRepository.GetScheduledToProcessAsync(currentDate);

            foreach (var transfer in transfers)
            {
                transfer.StartProcessing();

                try
                {
                    var source = await _accountRepository.GetByIdForUpdateAsync(transfer.SourceAccountId);

                    var destination = await _accountRepository.GetByIdForUpdateAsync(transfer.DestinationAccountId);

                    if (source is null || destination is null)
                    {
                        throw new InvalidOperationException("Uma das contas não foi encontrada.");
                    }

                    if (source.Status != AccountStatus.Active || destination.Status != AccountStatus.Active)
                    {
                        throw new InvalidOperationException("Uma das contas não está ativa.");
                    }

                    await _limitService.ValidateAsync(
                        source.Id,
                        transfer.Amount,
                        currentDate);

                    source.Debit(transfer.Amount);
                    destination.Credit(transfer.Amount);

                    transfer.Complete();

                    await _attemptRepository.AddAsync(
                        new TransferAttempt(
                            source.Id,
                            transfer.Amount,
                            true,
                            currentDate));
                }
                catch (InvalidOperationException)
                {
                    transfer.Fail();

                    await _attemptRepository.AddAsync(
                        new TransferAttempt(
                            transfer.SourceAccountId,
                            transfer.Amount,
                            false,
                            currentDate));
                }

                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
