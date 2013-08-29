using System;
using System.ComponentModel.DataAnnotations;

namespace BycicleShop.Models.AdminModels
{
    public class ProductStatisticModel
    {
        [Required]
        public int? ProductId { get; set; }

        [Required]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Required]
        [Display(Name = "Count")]
        public int Count { get; set; }

        [Required]
        [Display(Name = "Sent Date")]
        public DateTime? SentDate { get; set; }

        [Display(Name = "Receive Date")]
        public DateTime? ReceiveDate { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }
    }
}