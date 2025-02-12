using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Web_Project.Models
{
    public class User :IdentityUser
    {

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 5)]
        public string ZipCode { get; set; }

        [Required]
        public string MobileNumber { get; set; }

        public string ProfilePicturePath { get; set; } // Add this property



    }
}
