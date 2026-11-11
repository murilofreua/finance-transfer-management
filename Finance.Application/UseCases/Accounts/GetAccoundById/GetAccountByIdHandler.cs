using Finance.Application.DTOs.Accounts;
using Finance.Application.Interfaces.Repositories;

namespace Finance.Application.UseCases.Accounts.GetAccountById
{
    public class GetAccountByIdHandler
    {
        private readonly IAccountRepository _accountRepository;

        public GetAccountByIdHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<AccountResponse?> Handle(GetAccountByIdQuery query)
        {
            var account = await _accountRepository.GetByIdAsync(query.AccountId);

            if (account == null)
            {
                return null;
            }

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
