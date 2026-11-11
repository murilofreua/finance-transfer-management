using Finance.Application.Interfaces.Repositories;
using Finance.Application.Services;
using Finance.Domain.Entities;
using Finance.Domain.Enums;
using Moq;
using Xunit;

namespace Finance.UnitTests
{
    public class TransferLimitServiceTests
    {
        private readonly Mock<ITransferLimitRepository> _limitRepository;
        private readonly Mock<ITransferAttemptRepository> _attemptRepository;
        private readonly TransferLimitService _service;

        public TransferLimitServiceTests()
        {
            _limitRepository = new Mock<ITransferLimitRepository>();
            _attemptRepository = new Mock<ITransferAttemptRepository>();

            _service = new TransferLimitService(
                _limitRepository.Object,
                _attemptRepository.Object);
        }

        [Theory]
        [InlineData("2026-11-10T12:00:00Z", TransferPeriod.Day)]
        [InlineData("2026-11-11T02:00:00Z", TransferPeriod.Night)]
        [InlineData("2026-11-10T05:00:00Z", TransferPeriod.Night)]
        public void GetCurrentPeriod_ShouldReturnCorrectPeriod(string date, TransferPeriod expected)
        {
            var result = _service.GetCurrentPeriod(DateTime.Parse(date).ToUniversalTime());

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task ValidateAsync_ShouldPass_WhenLimitsAreNotExceeded()
        {
            var accountId = Guid.NewGuid();
            var currentDate = new DateTime(2026, 11, 10, 15, 0, 0, DateTimeKind.Utc);

            _limitRepository
                .Setup(x => x.GetByPeriodAsync(TransferPeriod.Day))
                .ReturnsAsync(new TransferLimit(TransferPeriod.Day, 5000, 10));

            _attemptRepository
                .Setup(x => x.GetByAccountAndPeriodAsync(
                    accountId,
                    currentDate.AddHours(-1),
                    currentDate))
                .ReturnsAsync(new List<TransferAttempt>());

            await _service.ValidateAsync(accountId, 1000, currentDate);
        }

        [Fact]
        public async Task ValidateAsync_ShouldThrow_WhenLimitIsNotConfigured()
        {
            _limitRepository
                .Setup(x => x.GetByPeriodAsync(It.IsAny<TransferPeriod>()))
                .ReturnsAsync((TransferLimit)null!);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.ValidateAsync(
                    Guid.NewGuid(),
                    100,
                    DateTime.UtcNow));
        }

        [Fact]
        public async Task ValidateAsync_ShouldThrow_WhenMaxAttemptsAreExceeded()
        {
            var accountId = Guid.NewGuid();
            var currentDate = new DateTime(2026, 11, 10, 15, 0, 0, DateTimeKind.Utc);

            _limitRepository
                .Setup(x => x.GetByPeriodAsync(TransferPeriod.Day))
                .ReturnsAsync(new TransferLimit(TransferPeriod.Day, 5000, 2));

            var attempts = new List<TransferAttempt>
            {
                new(accountId, 100, true, currentDate.AddMinutes(-30)),
                new(accountId, 200, false, currentDate.AddMinutes(-20))
            };

            _attemptRepository
                .Setup(x => x.GetByAccountAndPeriodAsync(
                    accountId,
                    currentDate.AddHours(-1),
                    currentDate))
                .ReturnsAsync(attempts);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.ValidateAsync(accountId, 100, currentDate));
        }

        [Fact]
        public async Task ValidateAsync_ShouldThrow_WhenMaxAmountIsExceeded()
        {
            var accountId = Guid.NewGuid();
            var currentDate = new DateTime(2026, 11, 10, 15, 0, 0, DateTimeKind.Utc);

            _limitRepository
                .Setup(x => x.GetByPeriodAsync(TransferPeriod.Day))
                .ReturnsAsync(new TransferLimit(TransferPeriod.Day, 1000, 10));

            var attempts = new List<TransferAttempt>
            {
                new(accountId, 700, true, currentDate.AddMinutes(-30))
            };

            _attemptRepository
                .Setup(x => x.GetByAccountAndPeriodAsync(
                    accountId,
                    currentDate.AddHours(-1),
                    currentDate))
                .ReturnsAsync(attempts);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.ValidateAsync(accountId, 400, currentDate));
        }

        [Fact]
        public async Task ValidateAsync_ShouldIgnoreFailedAttemptsWhenCalculatingAmount()
        {
            var accountId = Guid.NewGuid();
            var currentDate = new DateTime(2026, 11, 10, 15, 0, 0, DateTimeKind.Utc);

            _limitRepository
                .Setup(x => x.GetByPeriodAsync(TransferPeriod.Day))
                .ReturnsAsync(new TransferLimit(TransferPeriod.Day, 1000, 10));

            var attempts = new List<TransferAttempt>
            {
                new(accountId, 900, false, currentDate.AddMinutes(-30))
            };

            _attemptRepository
                .Setup(x => x.GetByAccountAndPeriodAsync(
                    accountId,
                    currentDate.AddHours(-1),
                    currentDate))
                .ReturnsAsync(attempts);

            await _service.ValidateAsync(accountId, 500, currentDate);
        }
    }
}
