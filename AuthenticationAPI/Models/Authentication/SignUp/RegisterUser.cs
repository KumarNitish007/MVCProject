using System.ComponentModel.DataAnnotations;

namespace AuthenticationAPI.Models.Authentication.SignUp
{
    public class RegisterUser
    {
      
        [Display(Name = "Username")]
        [Required(ErrorMessage = " User Name is Required")]
        public string? UserName { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is Required")]
        public string? Email { get; set; }

        [Required (ErrorMessage = "Password is Required")]
        public string? password { get; set; }
    }
}
