using System.ComponentModel.DataAnnotations;

namespace BycicleShop.Models.OrderModels
{
    public class OrderReceivedModel
    {
        [Required]
        public int OrderId { get; set; }
        [Display(Name = "Review")]
        public string Review { get; set; }
    }
}