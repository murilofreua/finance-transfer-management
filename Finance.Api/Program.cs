using Finance.Api.BackgroundServices;
using Finance.Api.Middlewares;
using Finance.Application.Interfaces.Repositories;
using Finance.Application.Interfaces.UnitOfWork;
using Finance.Application.Services;
using Finance.Application.UseCases.Accounts.CreateAccount;
using Finance.Application.UseCases.Accounts.GetAccountById;
using Finance.Application.UseCases.Accounts.GetAllAccounts;
using Finance.Application.UseCases.Transfers.CancelTransfer;
using Finance.Application.UseCases.Transfers.CreateTransfer;
using Finance.Application.UseCases.Transfers.GetAllTransfers;
using Finance.Application.UseCases.Transfers.ProcessScheduledTransfers;
using Finance.Infrastructure.Data;
using Finance.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("VueFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<FinanceDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITransferRepository, TransferRepository>();
builder.Services.AddScoped<ITransferAttemptRepository, TransferAttemptRepository>();
builder.Services.AddScoped<ITransferLimitRepository, TransferLimitRepository>();

builder.Services.AddScoped<GetAccountByIdHandler>();
builder.Services.AddScoped<GetAllAccountsHandler>();
builder.Services.AddScoped<CreateAccountHandler>();
builder.Services.AddScoped<GetAllTransfersHandler>();
builder.Services.AddScoped<CreateTransferHandler>();
builder.Services.AddScoped<CancelTransferHandler>();
builder.Services.AddScoped<ProcessScheduledTransfersHandler>();

builder.Services.AddScoped<TransferLimitService>();

builder.Services.AddHostedService<ScheduledTransferWorker>();

builder.Services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<FinanceDbContext>());

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();

    await dbContext.Database.MigrateAsync();

    await dbContext.Database.ExecuteSqlRawAsync("""
        IF NOT EXISTS (SELECT 1 FROM TransferLimits WHERE Period = 'Day')
        BEGIN
            INSERT INTO TransferLimits (Id, Period, MaxAmountPerHour, MaxAttemptsPerHour)
            VALUES (NEWID(), 'Day', 5000.00, 10);
        END;

        IF NOT EXISTS (SELECT 1 FROM TransferLimits WHERE Period = 'Night')
        BEGIN
            INSERT INTO TransferLimits (Id, Period, MaxAmountPerHour, MaxAttemptsPerHour)
            VALUES (NEWID(), 'Night', 2000.00, 5);
        END;
        """);
}

app.UseCors("VueFrontend");

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }