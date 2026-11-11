using System.Net;
using System.Net.Http.Json;
using Finance.Application.DTOs.Accounts;
using Xunit;

namespace Finance.IntegrationTests
{
    public class AccountIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public AccountIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task CreateAccount_ShouldReturnCreated_WhenRequestIsValid()
        {
            await _factory.InitializeDatabaseAsync();

            using var client = _factory.CreateClient();

            var request = new CreateAccountRequest
            {
                PersonName = "João Silva",
                Balance = 1000,
                OverdraftLimit = 500
            };

            var response = await client.PostAsJsonAsync(
                "/api/accounts",
                request);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var account = await response.Content
                .ReadFromJsonAsync<AccountResponse>();

            Assert.NotNull(account);
            Assert.NotEqual(Guid.Empty, account.Id);
            Assert.Equal(request.PersonName, account.PersonName);
            Assert.Equal(request.Balance, account.Balance);
            Assert.Equal(request.OverdraftLimit, account.OverdraftLimit);
            Assert.Equal("Active", account.Status);
        }

        [Fact]
        public async Task GetAccount_ShouldReturnAccount_WhenAccountExists()
        {
            await _factory.InitializeDatabaseAsync();

            using var client = _factory.CreateClient();

            var request = new CreateAccountRequest
            {
                PersonName = "Maria Silva",
                Balance = 2000,
                OverdraftLimit = 1000
            };

            var createResponse = await client.PostAsJsonAsync(
                "/api/accounts",
                request);

            var createdAccount = await createResponse.Content
                .ReadFromJsonAsync<AccountResponse>();

            var response = await client.GetAsync(
                $"/api/accounts/{createdAccount!.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var account = await response.Content
                .ReadFromJsonAsync<AccountResponse>();

            Assert.NotNull(account);
            Assert.Equal(createdAccount.Id, account.Id);
            Assert.Equal(request.PersonName, account.PersonName);
            Assert.Equal(request.Balance, account.Balance);
        }
    }
}
