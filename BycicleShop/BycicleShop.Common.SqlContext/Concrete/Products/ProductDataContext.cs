﻿using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using BycicleShop.Common.SqlContext.Entities;

namespace BycicleShop.Common.SqlContext.Concrete
{
    public class ProductDataContext
    {
        private AdventureWorks _dataContext = new AdventureWorks();

        public DbSet<Product> Entity
        {
            get { return _dataContext.Products; }
        }

        private Product Get(int id)
        {
            return Entity.First(x => x.ProductID == id);
        }

        public Product Get(string name)
        {
            return Entity.First(x => x.Name == name);
        }

        public void SaveChanges()
        {
            try
            {
                _dataContext.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (
                    var validationError in
                        dbEx.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors))
                {
                    Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName,
                                           validationError.ErrorMessage);
                }
            }
            catch (DbUpdateException dbEx)
            {
                var s = dbEx.ToString();
            }
        }
    }
}
