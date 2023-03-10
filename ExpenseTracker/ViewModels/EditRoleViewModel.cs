using ExpenseTracker.Models;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.ViewModels
{
    public class EditRoleViewModel
    {        
        public string Id { get; set; }
        [Required(ErrorMessage = "Role Name is required")]
        public string RoleName { get; set; }
        public List<Users> Users { get; set; }
    }
}
