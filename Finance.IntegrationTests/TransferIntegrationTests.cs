using System.Net;
using System.Net.Http.Json;
using Finance.Application.DTOs.Accounts;
using Finance.Application.DTOs.Transfers;
using Finance.Domain.Entities;
using Finance.Domain.Enums;
using Finance.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Finance.IntegrationTests
{
    public class TransferIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public TransferIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task CreateTransfer_ShouldComplete_WhenRequestIsValid()
        {
            await _factory.InitializeDatabaseAsync();

            using var client = _factory.CreateClient();

            // Configura os limites de transferência
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider
                    .GetRequiredService<FinanceDbContext>();

                if (!await dbContext.TransferLimits.AnyAsync())
                {
                    dbContext.TransferLimits.AddRange(
                        new TransferLimit(TransferPeriod.Day, 5000, 10),
                        new TransferLimit(TransferPeriod.Night, 2000, 5)
                    );

                    await dbContext.SaveChangesAsync();
                }
            }

            // Cria a conta de origem
            var sourceRequest = new CreateAccountRequest
            {
                PersonName = "João Silva",
                Balance = 1000,
                OverdraftLimit = 500
            };

            var sourceResponse = await client.PostAsJsonAsync(
                "/api/accounts", sourceRequest);

            var source = await sourceResponse.Content
                .ReadFromJsonAsync<AccountResponse>();

            // Cria a conta de destino
            var destinationRequest = new CreateAccountRequest
            {
                PersonName = "Maria Silva",
                Balance = 500,
                OverdraftLimit = 300
            };

            var destinationResponse = await client.PostAsJsonAsync(
                "/api/accounts", destinationRequest);

            var destination = await destinationResponse.Content
                .ReadFromJsonAsync<AccountResponse>();

            // Executa a transferência
            var transferRequest = new CreateTransferRequest
            {
                SourceAccountId = source!.Id,
                DestinationAccountId = destination!.Id,
                Amount = 300
            };

            var response = await client.PostAsJsonAsync(
                "/api/transfers", transferRequest);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var transfer = await response.Content
                .ReadFromJsonAsync<TransferResponse>();

            Assert.NotNull(transfer);
            Assert.Equal("Completed", transfer.Status);
            Assert.Equal(300, transfer.Amount);

            // Consulta os saldos atualizados
            var updatedSource = await client.GetFromJsonAsync<AccountResponse>(
                $"/api/accounts/{source.Id}");

            var updatedDestination = await client.GetFromJsonAsync<AccountResponse>(
                $"/api/accounts/{destination.Id}");

            Assert.Equal(700, updatedSource!.Balance);
            Assert.Equal(800, updatedDestination!.Balance);
        }

        [Fact]
        public async Task CreateTransfer_ShouldRemainScheduled_WhenScheduledAtIsFuture()
        {
            await _factory.InitializeDatabaseAsync();

            using var client = _factory.CreateClient();

            // Cria as contas
            var sourceRequest = new CreateAccountRequest
            {
                PersonName = "João Silva",
                Balance = 1000,
                OverdraftLimit = 500
            };

            var sourceResponse = await client.PostAsJsonAsync(
                "/api/accounts", sourceRequest);

            var source = await sourceResponse.Content
                .ReadFromJsonAsync<AccountResponse>();

            var destinationRequest = new CreateAccountRequest
            {
                PersonName = "Maria Silva",
                Balance = 500,
                OverdraftLimit = 300
            };

            var destinationResponse = await client.PostAsJsonAsync(
                "/api/accounts", destinationRequest);

            var destination = await destinationResponse.Content
                .ReadFromJsonAsync<AccountResponse>();

            // Agenda a transferência
            var transferRequest = new CreateTransferRequest
            {
                SourceAccountId = source!.Id,
                DestinationAccountId = destination!.Id,
                Amount = 300,
                ScheduledAt = DateTime.UtcNow.AddDays(1)
            };

            var response = await client.PostAsJsonAsync(
                "/api/transfers", transferRequest);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var transfer = await response.Content
                .ReadFromJsonAsync<TransferResponse>();

            Assert.NotNull(transfer);
            Assert.Equal("Scheduled", transfer.Status);

            // Verifica se os saldos permaneceram iguais
            var updatedSource = await client.GetFromJsonAsync<AccountResponse>(
                $"/api/accounts/{source.Id}");

            var updatedDestination = await client.GetFromJsonAsync<AccountResponse>(
                $"/api/accounts/{destination.Id}");

            Assert.Equal(1000, updatedSource!.Balance);
            Assert.Equal(500, updatedDestination!.Balance);
        }

        [Fact]
        public async Task CancelTransfer_ShouldReturnNoContent_WhenTransferIsScheduled()
        {
            await _factory.InitializeDatabaseAsync();

            using var client = _factory.CreateClient();

            // Cria as contas
            var sourceResponse = await client.PostAsJsonAsync(
                "/api/accounts",
                new CreateAccountRequest
                {
                    PersonName = "João Silva",
                    Balance = 1000,
                    OverdraftLimit = 500
                });

            var source = await sourceResponse.Content
                .ReadFromJsonAsync<AccountResponse>();

            var destinationResponse = await client.PostAsJsonAsync(
                "/api/accounts",
                new CreateAccountRequest
                {
                    PersonName = "Maria Silva",
                    Balance = 500,
                    OverdraftLimit = 300
                });

            var destination = await destinationResponse.Content
                .ReadFromJsonAsync<AccountResponse>();

            // Agenda a transferência
            var transferResponse = await client.PostAsJsonAsync(
                "/api/transfers",
                new CreateTransferRequest
                {
                    SourceAccountId = source!.Id,
                    DestinationAccountId = destination!.Id,
                    Amount = 300,
                    ScheduledAt = DateTime.UtcNow.AddDays(1)
                });

            var transfer = await transferResponse.Content
                .ReadFromJsonAsync<TransferResponse>();

            // Cancela a transferência
            var cancelResponse = await client.DeleteAsync(
                $"/api/transfers/{transfer!.Id}");

            Assert.Equal(HttpStatusCode.NoContent, cancelResponse.StatusCode);

            using var scope = _factory.Services.CreateScope();

            var dbContext = scope.ServiceProvider
                .GetRequiredService<FinanceDbContext>();

            var cancelledTransfer = await dbContext.Transfers
                .AsNoTracking()
                .SingleAsync(x => x.Id == transfer.Id);

            Assert.Equal(TransferStatus.Cancelled, cancelledTransfer.Status);
        }
    }
}
