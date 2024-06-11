using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MVCProject.Models
{
    public class ProfileViewModel
    {
        
        public string Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string Address { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        public string ProfilePicture { get; set; }

        [Display(Name = "Profile Picture")]
        public IFormFile ProfilePictureFile { get; set; } = null;
    }
}
