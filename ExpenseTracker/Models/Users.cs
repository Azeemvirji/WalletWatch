using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models
{
    [NotMapped]
    public class Users
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
