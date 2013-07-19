using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    }
}
