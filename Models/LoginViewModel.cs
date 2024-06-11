using System.ComponentModel.DataAnnotations;

namespace MVCProject.Models
{
	public class LoginViewModel
	{
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; } = true;
    }
}
