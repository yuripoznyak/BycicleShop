using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using BycicleShop.Common.SqlContext.Entities;

namespace BycicleShop.Common.SqlContext.Concrete
{
    public class OrderDataContext
    {
        private AdventureWorks _dataContext = new AdventureWorks();

        public DbSet<Order> Entity
        {
            get { return _dataContext.Orders; }
        }

        private Order Get(int id)
        {
            return Entity.First(x => x.OrderId == id);
        }

        public ICollection<Order> GetOrders(int userId)
        {
            return Entity.Where(x => x.User.UserId == userId).ToList();
        }
        
        public bool AddOrder(Order order)
        {
            try
            {
                _dataContext.Orders.Add(order);
                _dataContext.SaveChanges();
                return true;
            }
            catch (DbUpdateException exception)
            {
                var s = exception.ToString();
                return false;
            }
        }

        public int? GetLastOrderId(int userId)
        {
            return Entity.OrderByDescending(x => x.OrderId).FirstOrDefault().OrderId;
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
    }
}
