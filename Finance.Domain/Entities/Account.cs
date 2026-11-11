using Finance.Domain.Enums;

namespace Finance.Domain.Entities
{
    public class Account
    {
        public Guid Id { get; private set; }

        public string PersonName { get; private set; }

        public decimal Balance { get; private set; }

        public decimal OverdraftLimit { get; private set; }

        public AccountStatus Status { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public byte[] RowVersion { get; private set; } = [];

        protected Account() { PersonName = string.Empty; }

        public Account(string personName, decimal balance, decimal overdraftLimit, AccountStatus status, DateTime createdAt)
        {
            if (string.IsNullOrWhiteSpace(personName))
            {
                throw new ArgumentException("O nome da pessoa não foi informado.");
            }

            if (balance < 0)
            {
                throw new ArgumentException("O saldo inicial não pode ser negativo.");
            }

            if (overdraftLimit < 0)
            {
                throw new ArgumentException("O limite de cheque especial não pode ser negativo.");
            }

            if (!Enum.IsDefined(status))
            {
                throw new ArgumentException("O status da conta é inválido.");
            }

            Id = Guid.NewGuid();
            PersonName = personName;
            Balance = balance;
            OverdraftLimit = overdraftLimit;
            Status = status;
            CreatedAt = createdAt;
        }


        public void Debit(decimal amount)
        {
            if (Status != AccountStatus.Active)
            {
                throw new InvalidOperationException("A conta de origem não está ativa.");
            }

            if (amount <= 0)
            {
                throw new ArgumentException("O valor do débito deve ser maior que zero.");
            }

            if (Balance - amount < -OverdraftLimit)
            {
                throw new InvalidOperationException("Saldo e limite de cheque especial insuficientes.");
            }

            Balance -= amount;
        }

        public void Credit(decimal amount)
        {
            if (Status != AccountStatus.Active)
            {
                throw new InvalidOperationException("A conta de destino não está ativa.");
            }

            if (amount <= 0)
            {
                throw new ArgumentException("O valor do crédito deve ser maior que zero.");
            }

            Balance += amount;
        }
    }
}
