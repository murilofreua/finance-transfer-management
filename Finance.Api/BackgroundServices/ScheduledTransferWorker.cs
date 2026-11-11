using Finance.Application.UseCases.Transfers.ProcessScheduledTransfers;

namespace Finance.Api.BackgroundServices
{
    public class ScheduledTransferWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ScheduledTransferWorker> _logger;

        public ScheduledTransferWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<ScheduledTransferWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var handler = scope.ServiceProvider.GetRequiredService<ProcessScheduledTransfersHandler>();

                    await handler.HandleAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar transferências agendadas.");
                }

                await Task.Delay(
                    TimeSpan.FromSeconds(30),
                    stoppingToken);
            }
        }
    }
}
