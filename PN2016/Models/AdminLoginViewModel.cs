using System.ComponentModel.DataAnnotations;

namespace PN2016.Models
{
    public class AdminLoginViewModel
    {
        [Required]
        [MaxLength(50)]
        [Display(Name = "UserName", GroupName = "Primary")]
        public string UserName { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Password", GroupName = "Primary")]
        public string Password { get; set; }
    }
}