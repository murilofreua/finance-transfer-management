using Finance.Application.DTOs.Accounts;
using Finance.Application.Interfaces.Repositories;
using Finance.Application.Interfaces.UnitOfWork;
using Finance.Domain.Entities;
using Finance.Domain.Enums;

namespace Finance.Application.UseCases.Accounts.CreateAccount
{
    public class CreateAccountHandler
    {
        private readonly IAccountRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateAccountHandler(IAccountRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<AccountResponse> HandleAsync(CreateAccountRequest request)
        {
            var account = new Account(
                request.PersonName,
                request.Balance,
                request.OverdraftLimit,
                AccountStatus.Active,
                DateTime.UtcNow
            );

            await _repository.AddAsync(account);
            await _unitOfWork.SaveChangesAsync();

            return new AccountResponse
            {
                Id = account.Id,
                PersonName = account.PersonName,
                Balance = account.Balance,
                OverdraftLimit = account.OverdraftLimit,
                Status = account.Status.ToString(),
                CreatedAt = account.CreatedAt
            };
        }
    }
}
