using Finance.Application.DTOs.Accounts;
using Finance.Application.Interfaces.Repositories;

namespace Finance.Application.UseCases.Accounts.GetAllAccounts
{
    public class GetAllAccountsHandler
    {
        private readonly IAccountRepository _accountRepository;

        public GetAllAccountsHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<List<AccountResponse>> HandleAsync()
        {
            var accounts = await _accountRepository.GetAllAsync();

            return accounts.Select(account => new AccountResponse
            {
                Id = account.Id,
                PersonName = account.PersonName,
                Balance = account.Balance,
                OverdraftLimit = account.OverdraftLimit,
                Status = account.Status.ToString(),
                CreatedAt = account.CreatedAt
            }).ToList();
        }
    }
}
