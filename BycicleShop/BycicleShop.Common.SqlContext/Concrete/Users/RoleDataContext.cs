﻿using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using BycicleShop.Common.SqlContext.Entities;

namespace BycicleShop.Common.SqlContext.Concrete
{
    public class RoleDataContext
    {
        private AdventureWorks _dataContext = new AdventureWorks();

        public DbSet<Role> Entity
        {
            get { return _dataContext.Roles; }
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