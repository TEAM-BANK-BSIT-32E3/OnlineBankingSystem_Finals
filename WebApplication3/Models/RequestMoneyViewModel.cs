namespace WebApplication3.Models
{
    public class RequestMoneyViewModel
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string RecipientAccountNumber { get; set; } // This will capture the recipient's account number
    }

}
