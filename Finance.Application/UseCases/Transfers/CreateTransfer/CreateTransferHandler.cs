using Finance.Application.DTOs.Transfers;
using Finance.Application.Interfaces.Repositories;
using Finance.Application.Interfaces.UnitOfWork;
using Finance.Application.Services;
using Finance.Domain.Entities;
using Finance.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.UseCases.Transfers.CreateTransfer
{
    public class CreateTransferHandler
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransferRepository _transferRepository;
        private readonly ITransferAttemptRepository _transferAttemptRepository;
        private readonly IUnitOfWork _unitOfWork;

        private readonly TransferLimitService _transferLimitService;

        public CreateTransferHandler(
            IAccountRepository accountRepository,
            ITransferRepository transferRepository,
            ITransferAttemptRepository transferAttemptRepository,
            IUnitOfWork unitOfWork,
            TransferLimitService transferLimitService)
        {
            _accountRepository = accountRepository;
            _transferRepository = transferRepository;
            _transferAttemptRepository = transferAttemptRepository;
            _unitOfWork = unitOfWork;
            _transferLimitService = transferLimitService;
        }

        public async Task<TransferResponse> HandleAsync(CreateTransferRequest request)
        {
            var sourceAccount = await GetAccountAsync(request.SourceAccountId);
            var destinationAccount = await GetAccountAsync(request.DestinationAccountId);

            ValidateAccounts(sourceAccount, destinationAccount, request.Amount);

            var createdAt = DateTime.UtcNow;

            var transfer = new Transfer(
                sourceAccount.Id,
                destinationAccount.Id,
                request.Amount,
                request.ScheduledAt,
                createdAt);

            if (request.ScheduledAt.HasValue)
                return await ScheduleTransferAsync(transfer);

            return await ExecuteTransferAsync(
                transfer,
                sourceAccount,
                destinationAccount);
        }

        private async Task<Account> GetAccountAsync(Guid accountId)
        {
            var account = await _accountRepository.GetByIdForUpdateAsync(accountId);

            return account ?? throw new InvalidOperationException("Conta não encontrada.");
        }

        private void ValidateAccounts(Account source, Account destination, decimal amount)
        {
            if (source.Id == destination.Id)
            {
                throw new InvalidOperationException("As contas devem ser diferentes.");
            }

            if (amount <= 0)
            {
                throw new ArgumentException("O valor deve ser maior que zero.");
            }

            if (source.Status != AccountStatus.Active)
            {
                throw new InvalidOperationException("A conta de origem não está ativa.");
            }

            if (destination.Status != AccountStatus.Active)
            {
                throw new InvalidOperationException("A conta de destino não está ativa.");
            }
        }

        private async Task<TransferResponse> ScheduleTransferAsync(Transfer transfer)
        {
            await _transferRepository.AddAsync(transfer);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(transfer);
        }

        private async Task<TransferResponse> ExecuteTransferAsync(Transfer transfer, Account source, Account destination)
        {
            var createdAt = DateTime.UtcNow;

            try
            {
                await _transferLimitService.ValidateAsync(
                    source.Id,
                    transfer.Amount,
                    createdAt);
            }
            catch (InvalidOperationException)
            {
                await RegisterAttemptAsync(source.Id, transfer.Amount, false, createdAt);
                throw;
            }

            source.Debit(transfer.Amount);
            destination.Credit(transfer.Amount);

            transfer.Complete();

            await _transferRepository.AddAsync(transfer);
            await RegisterAttemptAsync(source.Id, transfer.Amount, true, createdAt);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new DbUpdateConcurrencyException("A conta foi alterada por outra operação. Tente novamente.");
            }

            return MapToResponse(transfer);
        }

        private async Task RegisterAttemptAsync(Guid accountId, decimal amount, bool wasSuccessful, DateTime createdAt)
        {
            await _transferAttemptRepository.AddAsync(
                new TransferAttempt(accountId, amount, wasSuccessful, createdAt));
        }

        private TransferResponse MapToResponse(Transfer transfer)
        {
            return new TransferResponse
            {
                Id = transfer.Id,
                SourceAccountId = transfer.SourceAccountId,
                DestinationAccountId = transfer.DestinationAccountId,
                Amount = transfer.Amount,
                Status = transfer.Status.ToString(),
                CreatedAt = transfer.CreatedAt,
                ScheduledAt = transfer.ScheduledAt.HasValue ? DateTime.SpecifyKind(transfer.ScheduledAt.Value, DateTimeKind.Utc) : null
            };
        }
    }
}
