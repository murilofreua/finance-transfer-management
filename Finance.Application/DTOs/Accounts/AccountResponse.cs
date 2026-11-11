namespace Finance.Application.DTOs.Accounts
{
    public class AccountResponse
    {
        public Guid Id { get; set; }

        public string PersonName { get; set; }

        public decimal Balance { get; set; }

        public decimal OverdraftLimit { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
