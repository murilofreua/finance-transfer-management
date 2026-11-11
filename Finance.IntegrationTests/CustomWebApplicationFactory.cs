using Finance.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Finance.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<FinanceDbContext>>();

                services.AddDbContext<FinanceDbContext>(options =>
                    options.UseSqlServer(
                        "Server=localhost;Database=FinanceDb_IntegrationTests;Trusted_Connection=True;TrustServerCertificate=True;"));

                services.RemoveAll<IHostedService>();
            });
        }

        public async Task InitializeDatabaseAsync()
        {
            using var scope = Services.CreateScope();

            var dbContext = scope.ServiceProvider
                .GetRequiredService<FinanceDbContext>();

            await dbContext.Database.MigrateAsync();
        }
    }
}
