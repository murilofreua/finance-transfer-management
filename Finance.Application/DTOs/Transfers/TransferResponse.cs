namespace Finance.Application.DTOs.Transfers
{
    public class TransferResponse
    {
        public Guid Id { get; set; }

        public Guid SourceAccountId { get; set; }

        public Guid DestinationAccountId { get; set; }

        public decimal Amount { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? ScheduledAt { get; set; }
    }
}
