// Deposit.cs (Model)
namespace WebApplication3.Models
{
    public class Deposit
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime DepositDate { get; set; }
    }
}
