using Finance.Domain.Enums;
using Finance.Application.Interfaces.Repositories;

namespace Finance.Application.Services
{
    public class TransferLimitService
    {
        private readonly ITransferLimitRepository _limitRepository;
        private readonly ITransferAttemptRepository _attemptRepository;

        public TransferLimitService(ITransferLimitRepository limitRepository, ITransferAttemptRepository attemptRepository)
        {
            _limitRepository = limitRepository;
            _attemptRepository = attemptRepository;
        }

        public TransferPeriod GetCurrentPeriod(DateTime dateTime)
        {
            var brazilTimeZone = TimeZoneInfo.FindSystemTimeZoneById(
                OperatingSystem.IsWindows()
                    ? "E. South America Standard Time"
                    : "America/Sao_Paulo");

            var brazilTime = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.SpecifyKind(dateTime, DateTimeKind.Utc),
                brazilTimeZone);

            return brazilTime.Hour >= 6 && brazilTime.Hour < 22
                ? TransferPeriod.Day
                : TransferPeriod.Night;
        }

        public async Task ValidateAsync(Guid accountId, decimal amount, DateTime currentDate)
        {
            var period = GetCurrentPeriod(currentDate);

            var limit = await _limitRepository.GetByPeriodAsync(period);

            if (limit is null)
            {
                throw new InvalidOperationException("Limite de transferência não configurado.");
            }

            var startDate = currentDate.AddHours(-1);

            var attempts = await _attemptRepository.GetByAccountAndPeriodAsync(
                accountId,
                startDate,
                currentDate);

            if (attempts.Count >= limit.MaxAttemptsPerHour)
            {
                throw new InvalidOperationException("Limite de tentativas por hora excedido.");
            }

            var totalTransferred = attempts
                .Where(x => x.WasSuccessful)
                .Sum(x => x.Amount);

            if (totalTransferred + amount > limit.MaxAmountPerHour)
            {
                throw new InvalidOperationException("Limite de valor por hora excedido.");
            }
        }
    }
}
