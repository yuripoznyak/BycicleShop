using System;

namespace BycicleShop.Models.AdminModels
{
    public class OrderStatisticModel
    {
        public int OrderId { get; set; }

        public string Adress { get; set; }

        public decimal? TotalPrice { get; set; }

        public DateTime? SentDate { get; set; }

        public DateTime? ReceiveDate { get; set; }

        public string Username { get; set; }
    }
}