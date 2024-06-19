using System.ComponentModel.DataAnnotations;

namespace AuthenticationAPI.Models.Authentication.Login
{
    public class LoginModel
    {
        [Display(Name = "Username")]
        [Required(ErrorMessage = " User Name is Required")]
        public string? UserName { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is Required")]
        public string? password { get; set; }
    }
}
