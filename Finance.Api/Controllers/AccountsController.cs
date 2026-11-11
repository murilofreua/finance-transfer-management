using Finance.Application.DTOs.Accounts;
using Finance.Application.UseCases.Accounts.CreateAccount;
using Finance.Application.UseCases.Accounts.GetAccountById;
using Finance.Application.UseCases.Accounts.GetAllAccounts;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly GetAccountByIdHandler _getAccountByIdHandler;

        private readonly GetAllAccountsHandler _getAllAccountsHandler;

        private readonly CreateAccountHandler _createHandler;

        public AccountsController(GetAccountByIdHandler getAccountByIdHandler, GetAllAccountsHandler getAllAccountsHandler, CreateAccountHandler createHandler)
        {
            _getAccountByIdHandler = getAccountByIdHandler;
            _getAllAccountsHandler = getAllAccountsHandler;
            _createHandler = createHandler;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetAccountByIdQuery
            {
                AccountId = id
            };

            var result = await _getAccountByIdHandler.Handle(query);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _getAllAccountsHandler.HandleAsync();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAccountRequest request)
        {
            var result = await _createHandler.HandleAsync(request);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
    }
}
