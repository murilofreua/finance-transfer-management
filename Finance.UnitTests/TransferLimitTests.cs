using Finance.Domain.Entities;
using Finance.Domain.Enums;
using Xunit;

namespace Finance.UnitTests
{
    public class TransferLimitTests
    {
        [Fact]
        public void Constructor_ShouldCreateLimit_WhenValuesAreValid()
        {
            var limit = new TransferLimit(
                TransferPeriod.Day,
                5000,
                10);

            Assert.Equal(TransferPeriod.Day, limit.Period);
            Assert.Equal(5000, limit.MaxAmountPerHour);
            Assert.Equal(10, limit.MaxAttemptsPerHour);
            Assert.NotEqual(Guid.Empty, limit.Id);
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenMaxAmountIsZero()
        {
            Assert.Throws<ArgumentException>(() =>
                new TransferLimit(TransferPeriod.Day, 0, 10));
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenMaxAmountIsNegative()
        {
            Assert.Throws<ArgumentException>(() =>
                new TransferLimit(TransferPeriod.Day, -100, 10));
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenMaxAttemptsIsZero()
        {
            Assert.Throws<ArgumentException>(() =>
                new TransferLimit(TransferPeriod.Day, 5000, 0));
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenMaxAttemptsIsNegative()
        {
            Assert.Throws<ArgumentException>(() =>
                new TransferLimit(TransferPeriod.Day, 5000, -1));
        }
    }
}
