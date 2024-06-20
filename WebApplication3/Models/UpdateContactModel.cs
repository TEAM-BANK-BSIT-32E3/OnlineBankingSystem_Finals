using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models
{
    public class UpdateContactModel
    {
        [Required]
        [RegularExpression(@"^0\d{10}$", ErrorMessage = "Invalid contact number format. It should start with 0 and be 11 digits long.")]
        public string ContactNumber { get; set; }
    }
}
