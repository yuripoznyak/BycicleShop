using System;


namespace BycicleShop.Models.AdminModels
{
    public class OrderModel
    {
        public int Id { get; set; }
        public string Adress { get; set; }
        public DateTime? SentDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string Username { get; set; }
    }
}