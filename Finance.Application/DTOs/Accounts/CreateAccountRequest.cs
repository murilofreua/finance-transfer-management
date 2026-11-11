namespace Finance.Application.DTOs.Accounts
{
    public class CreateAccountRequest
    {
        public string PersonName { get; set; } = string.Empty;

        public decimal Balance { get; set; }

        public decimal OverdraftLimit { get; set; }
    }
}
