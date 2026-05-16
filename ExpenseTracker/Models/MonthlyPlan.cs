using System;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models
{
    public class MonthlyPlan
    {
        [Key]
        public int PlanId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Range(1, 12)]
        public int Month { get; set; }

        public int Year { get; set; }

        public float ExpectedIncome { get; set; }
    }
}
