using Finance.Domain.Enums;

namespace Finance.Domain.Entities
{
    public class TransferLimit
    {
        public Guid Id { get; private set; }

        public TransferPeriod Period { get; private set; }

        public decimal MaxAmountPerHour { get; private set; }

        public int MaxAttemptsPerHour { get; private set; }

        protected TransferLimit() { }

        public TransferLimit(TransferPeriod period, decimal maxAmountPerHour, int maxAttemptsPerHour)
        {
            if (maxAmountPerHour <= 0)
            {
                throw new ArgumentException("O limite deve ser maior que zero.");
            }

            if (maxAttemptsPerHour <= 0)
            {
                throw new ArgumentException("A quantidade de tentativas deve ser maior que zero.");
            }

            Id = Guid.NewGuid();
            Period = period;
            MaxAmountPerHour = maxAmountPerHour;
            MaxAttemptsPerHour = maxAttemptsPerHour;
        }
    }
}
