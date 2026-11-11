namespace Finance.Domain.Entities
{
    public class TransferAttempt
    {
        public Guid Id { get; private set; }

        public Guid AccountId { get; private set; }

        public decimal Amount { get; private set; }

        public bool WasSuccessful { get; private set; }

        public DateTime CreatedAt { get; private set; }

        protected TransferAttempt() { }

        public TransferAttempt(Guid accountId, decimal amount, bool wasSuccessful, DateTime createdAt)
        {
            if (accountId == Guid.Empty)
            {
                throw new ArgumentException("A conta não foi informada.");
            }

            if (amount <= 0)
            {
                throw new ArgumentException("O valor deve ser maior que zero.");
            }

            Id = Guid.NewGuid();
            AccountId = accountId;
            Amount = amount;
            WasSuccessful = wasSuccessful;
            CreatedAt = createdAt;
        }
    }
}
