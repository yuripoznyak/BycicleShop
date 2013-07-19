using System;

namespace BycicleShop.Models.ProductModels
{
    public class ProductExtendedModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Color { get; set; }
        public decimal? Weght { get; set; }
        public string Size { get; set; }
        public DateTime SellStartDate { get; set; }
        public DateTime? SellEndDate { get; set; }
        public string PhotoName { get; set; }
    }
}