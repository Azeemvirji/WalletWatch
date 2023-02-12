using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }
        public Guid UserId { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Range(1, int.MaxValue, ErrorMessage="Please select a category")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public float Amount { get; set; }
        [Column(TypeName = "nvarchar(75)")]
        public string? Note { get; set; }
        [Required]
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
