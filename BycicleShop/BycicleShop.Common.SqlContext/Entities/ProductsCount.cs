//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BycicleShop.Common.SqlContext.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class ProductsCount
    {
        public ProductsCount()
        {
            this.Products = new HashSet<Product>();
        }
    
        public int ProductsCountId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public int Count { get; set; }
        public Nullable<int> BasketId { get; set; }
    
        public virtual Basket Basket { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}