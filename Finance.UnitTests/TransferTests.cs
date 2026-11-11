using Finance.Domain.Entities;
using Finance.Domain.Enums;
using Xunit;

namespace Finance.UnitTests
{
    public class TransferTests
    {
        [Fact]
        public void Constructor_ShouldCreateImmediateTransferWithProcessingStatus()
        {
            var transfer = new Transfer(
                Guid.NewGuid(),
                Guid.NewGuid(),
                100,
                null,
                DateTime.UtcNow);

            Assert.Equal(TransferStatus.Processing, transfer.Status);
        }

        [Fact]
        public void Constructor_ShouldCreateScheduledTransfer()
        {
            var now = DateTime.UtcNow;

            var transfer = new Transfer(
                Guid.NewGuid(),
                Guid.NewGuid(),
                100,
                now.AddDays(1),
                now);

            Assert.Equal(TransferStatus.Scheduled, transfer.Status);
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenAccountsAreEqual()
        {
            var accountId = Guid.NewGuid();

            Assert.Throws<ArgumentException>(() =>
                new Transfer(
                    accountId,
                    accountId,
                    100,
                    null,
                    DateTime.UtcNow));
        }

        [Fact]
        public void Complete_ShouldChangeStatusToCompleted()
        {
            var transfer = new Transfer(
                Guid.NewGuid(),
                Guid.NewGuid(),
                100,
                null,
                DateTime.UtcNow);

            transfer.Complete();

            Assert.Equal(TransferStatus.Completed, transfer.Status);
        }

        [Fact]
        public void Cancel_ShouldChangeStatusToCancelled()
        {
            var now = DateTime.UtcNow;

            var transfer = new Transfer(
                Guid.NewGuid(),
                Guid.NewGuid(),
                100,
                now.AddDays(1),
                now);

            transfer.Cancel();

            Assert.Equal(TransferStatus.Cancelled, transfer.Status);
        }

        [Fact]
        public void Cancel_ShouldThrow_WhenTransferIsNotScheduled()
        {
            var transfer = new Transfer(
                Guid.NewGuid(),
                Guid.NewGuid(),
                100,
                null,
                DateTime.UtcNow);

            Assert.Throws<InvalidOperationException>(
                () => transfer.Cancel());
        }
    }
}
