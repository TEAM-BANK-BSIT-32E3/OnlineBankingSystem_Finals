namespace WebApplication3.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string ContactNumber { get; set; }

        public bool Contact_verified { get; set; } 


        public string Email { get; set; }
        public string Address { get; set; }
        public string Branch { get; set; }
        public string AccountType { get; set; }
        public string AccountNumber { get; set; }

         public decimal Balance { get; set; }

        public string Pin { get; set; }

        public string SecurityQuestion1 { get; set; }
        public string SecurityQuestion2 { get; set; }
        public string Answer1 { get; set; }

        public string Answer2 { get; set; }
    }
}
