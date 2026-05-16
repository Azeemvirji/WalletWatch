using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models
{
    public class Budget
    {
        [Key]
        public int BudgetId { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Range(1, 12)]
        public int Month { get; set; }

        public int Year { get; set; }

        public float? TargetPercentage { get; set; }
    }
}
