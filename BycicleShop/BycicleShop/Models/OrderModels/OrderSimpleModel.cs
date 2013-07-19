using System;

namespace BycicleShop.Models.OrderModels
{
    public class OrderSimpleModel
    {
        public int Id { get; set; }
        public bool Obtained { get; set; }
        public string Adress { get; set; }
        public DateTime? SentDate { get; set; }
    }
}