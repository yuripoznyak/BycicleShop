using System;
using System.ComponentModel.DataAnnotations;

namespace BycicleShop.Models.OrderModels
{
    public class OrderSimpleModel
    {
        [Required]
        public int Id { get; set; }

        [Display(Name = "Adress")]
        public string Adress { get; set; }

        public bool Received { get; set; }

        [Display(Name = "SentDate")]
        public DateTime? SentDate { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}