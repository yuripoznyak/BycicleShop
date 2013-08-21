using System.ComponentModel.DataAnnotations;

namespace BycicleShop.Models.AccountModels
{
    public class ChangePasswordModel
    {
        [Required]
        [Display(Name = "Old password")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
        [Required]
        [Display(Name = "New password")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Required]
        [Display(Name = "Confirm new password")]
        [DataType(DataType.Password)]
        public string ConfirmNewPassword { get; set; }
    }
}