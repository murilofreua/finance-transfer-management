using Finance.Domain.Entities;
using Finance.Domain.Enums;
using Xunit;

namespace Finance.UnitTests
{
    public class AccountTests
    {
        [Fact]
        public void Debit_ShouldDecreaseBalance_WhenAccountHasSufficientFunds()
        {
            var account = new Account(
                "João",
                1000,
                500,
                AccountStatus.Active,
                DateTime.UtcNow);

            account.Debit(300);

            Assert.Equal(700, account.Balance);
        }

        [Fact]
        public void Debit_ShouldUseOverdraft_WhenBalanceIsInsufficient()
        {
            var account = new Account(
                "João",
                100,
                500,
                AccountStatus.Active,
                DateTime.UtcNow);

            account.Debit(300);

            Assert.Equal(-200, account.Balance);
        }

        [Fact]
        public void Debit_ShouldThrow_WhenOverdraftLimitIsExceeded()
        {
            var account = new Account(
                "João",
                100,
                200,
                AccountStatus.Active,
                DateTime.UtcNow);

            Assert.Throws<InvalidOperationException>(
                () => account.Debit(400));
        }

        [Fact]
        public void Credit_ShouldIncreaseBalance()
        {
            var account = new Account(
                "João",
                100,
                200,
                AccountStatus.Active,
                DateTime.UtcNow);

            account.Credit(300);

            Assert.Equal(400, account.Balance);
        }
    }
}
