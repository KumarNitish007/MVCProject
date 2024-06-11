using System.ComponentModel.DataAnnotations;

namespace MVCProject.Models
{
	public class ForgotPasswordViewModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }
	}
}
