using System.ComponentModel.DataAnnotations;

namespace BycicleShop.Models.AccountModels
{
    public class UserProfileModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }
    }
}