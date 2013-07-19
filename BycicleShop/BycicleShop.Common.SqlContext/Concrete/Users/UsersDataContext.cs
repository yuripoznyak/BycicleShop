using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using BycicleShop.Common.SqlContext.Entities;

namespace BycicleShop.Common.SqlContext.Concrete
{
    public class UsersDataContext : IDisposable
    {
        private AdventureWorks _dataContext = new AdventureWorks();

        public DbSet<User> Entity
        {
            get { return _dataContext.Users; }
        }

        private User Get(int id)
        {
            return Entity.First(x => x.UserId == id);
        }

        public User Get(string name)
        {
            return Entity.First(x => x.UserName == name);
        }

        public void Dispose()
        {
            _dataContext.Dispose();
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
