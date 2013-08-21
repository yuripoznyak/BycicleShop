using System;
using System.ComponentModel.DataAnnotations;

namespace BycicleShop.Models.OrderModels
{
    public class OrderStepModel
    {
        [Required]
        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [Display(Name = "Adress")]
        public string Adress { get; set; }

        [Display(Name = "Sent Date")]
        public DateTime SentDate { get; set; }

        [Display(Name = "Comments")]
        public string Comments { get; set; }

        [Required]
        [Display(Name = "Credit Card")]
        public long CreditCard { get; set; }

        [Required]
        public int BasketId { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        
    }
}