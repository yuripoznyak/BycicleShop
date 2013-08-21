using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using BycicleShop.Common.SqlContext.Entities;

namespace BycicleShop.Common.SqlContext.Concrete
{
    public class ProductsCountDataContext
    {
        private AdventureWorks _dataContext = new AdventureWorks();

        public DbSet<ProductsCount> Entity
        {
            get { return _dataContext.ProductsCounts; }
        }

        public void SaveChanges()
        {
            try
            {
                _dataContext.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationError in dbEx.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors))
                {
                    Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                }
            }
        }

        public bool AddItem(int basketId, int productId, int count)
        {
            try
            {
                _dataContext.ProductsCounts.Add(new ProductsCount
                {
                    BasketId = basketId,
                    Count = count,
                    ProductId = productId,
                });
                _dataContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
