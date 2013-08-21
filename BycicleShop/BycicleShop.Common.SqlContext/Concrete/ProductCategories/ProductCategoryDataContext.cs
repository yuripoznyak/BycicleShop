using System.Data.Entity;
using System.Linq;
using BycicleShop.Common.SqlContext.Entities;

namespace BycicleShop.Common.SqlContext.Concrete
{
    public class ProductCategoryDataContext
    {
        private AdventureWorks _dataContext = new AdventureWorks();
        
        public DbSet<ProductCategory> Entity
        {
            get { return _dataContext.ProductCategories; }
        }

        private ProductCategory Get(int id)
        {
            return Entity.First(x => x.ProductCategoryID == id);
        }

        public ProductCategory Get(string name)
        {
            return Entity.First(x => x.Name == name);
        }

        public int GetId(string name)
        {
            return Entity.First(x => x.Name.ToUpper() == name.ToUpper()).ProductCategoryID;
        }
    }
}
