using System.ComponentModel.DataAnnotations;

namespace BycicleShop.Models.AdminModels
{
    public class NewUserModel
    {
        [Required]
        [Display(Name = "username")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [MaxLength(16)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}