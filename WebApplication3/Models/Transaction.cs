using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        [Required]
        [StringLength(100)]
        public string AccountName { get; set; }

        [Required]
        public string AccountNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string TransactionType { get; set; }

        [Required]
        [Range(0, Double.MaxValue)]
        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.Now;

        public string Description { get; set; }

        // Add additional properties for money requests
        public string RecipientAccountNumber { get; set; } // This is the account number of the recipient
        public string RequestStatus { get; set; } // Status of the money request (e.g., pending, accepted, rejected)


    }
}
