namespace Finance.Application.DTOs.Transfers
{
    public class CreateTransferRequest
    {
        public Guid SourceAccountId { get; set; }

        public Guid DestinationAccountId { get; set; }

        public decimal Amount { get; set; }

        public DateTime? ScheduledAt { get; set; }
    }
}
