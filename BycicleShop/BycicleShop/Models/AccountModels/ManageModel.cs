using System.ComponentModel.DataAnnotations;

namespace BycicleShop.Models.AccountModels
{
    public class ManageModel
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Adress")]
        public string Adress { get; set; }
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }
}