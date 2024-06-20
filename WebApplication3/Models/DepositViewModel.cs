namespace WebApplication3.ViewModels
{
    public class DepositViewModel
    {
        public string AccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } = "Deposit"; 
    }
}
