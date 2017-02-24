using System.ComponentModel.DataAnnotations;
namespace BBApp.Models
{
    public class RegisterViewModel : BaseEntity
    {
        [Required]
        [MinLength(2)]
        public string first_name { get; set; }
        [Required]
        [MinLength(2)]
        public string last_name { get; set; }
        [Required]
        [MinLength(4)]
        [EmailAddress]
        public string email { get; set; }
        [Required]
        [MinLength(8)]
        [Compare("confirm_password")]
        public string password { get; set; }
        [Required]
        public string confirm_password { get; set; }
    }
}