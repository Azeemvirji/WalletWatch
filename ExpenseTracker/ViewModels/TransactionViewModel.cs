using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.ViewModels
{
    public class TransactionViewModel
    {
        public int TransactionId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public float Amount { get; set; }

        [MaxLength(75, ErrorMessage = "Note cannot exceed 75 characters")]
        public string? Note { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
