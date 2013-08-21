using System.ComponentModel.DataAnnotations;

namespace BycicleShop.Models.AccountModels
{
    public class ChangeEmailModel
    {
        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [Display(Name = "Old Email")]
        [DataType(DataType.EmailAddress)]
        public string OldEmail { get; set; }
        [Required]
        [Display(Name = "New Email")]
        [DataType(DataType.EmailAddress)]
        public string NewEmail { get; set; }
    }
}