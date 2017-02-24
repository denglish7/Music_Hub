using System;
using System.ComponentModel.DataAnnotations;

namespace BBApp.Models {
    public class login : BaseEntity {
        [Key]
        [Required]
        [EmailAddress]
        public string loginEmail { get; set; }
        [Required]
        public string PasswordToCheck { get; set; }
    }
}