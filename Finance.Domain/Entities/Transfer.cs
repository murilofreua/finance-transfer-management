using Finance.Domain.Enums;

namespace Finance.Domain.Entities
{
    public class Transfer
    {
        public Guid Id { get; private set; }

        public Guid SourceAccountId { get; private set; }

        public Guid DestinationAccountId { get; private set; }

        public decimal Amount { get; private set; }

        public TransferStatus Status { get; private set; }

        public DateTime? ScheduledAt { get; private set; }

        public DateTime CreatedAt { get; private set; }

        protected Transfer() { }


        public Transfer(Guid sourceAccountId, Guid destinationAccountId, decimal amount, DateTime? scheduledAt, DateTime createdAt)
        {
            if (sourceAccountId == destinationAccountId)
            {
                throw new ArgumentException("As contas devem ser diferentes.");
            }

            if (amount <= 0)
            {
                throw new ArgumentException("O valor deve ser maior que zero.");
            }

            if (scheduledAt.HasValue && scheduledAt.Value <= createdAt)
            {
                throw new ArgumentException("A data de agendamento deve ser futura.");
            }

            Id = Guid.NewGuid();
            SourceAccountId = sourceAccountId;
            DestinationAccountId = destinationAccountId;
            Amount = amount;
            ScheduledAt = scheduledAt;
            CreatedAt = createdAt;
            Status = scheduledAt.HasValue ? TransferStatus.Scheduled : TransferStatus.Processing;
        }


        public void StartProcessing()
        {
            if (Status != TransferStatus.Scheduled)
                throw new InvalidOperationException(
                    "Apenas transferências agendadas podem ser processadas.");

            Status = TransferStatus.Processing;
        }

        public void Complete()
        {
            if (Status != TransferStatus.Processing)
                throw new InvalidOperationException(
                    "Apenas transferências em processamento podem ser concluídas.");

            Status = TransferStatus.Completed;
        }

        public void Fail()
        {
            if (Status != TransferStatus.Processing)
                throw new InvalidOperationException(
                    "Apenas transferências em processamento podem falhar.");

            Status = TransferStatus.Failed;
        }

        public void Cancel()
        {
            if (Status != TransferStatus.Scheduled)
                throw new InvalidOperationException(
                    "Apenas transferências agendadas podem ser canceladas.");

            Status = TransferStatus.Cancelled;
        }
    }
}
