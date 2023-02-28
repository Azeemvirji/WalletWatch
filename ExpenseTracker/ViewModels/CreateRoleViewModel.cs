using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.ViewModels
{
    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
    }
}
