using System.Data.Entity;
using System.Linq;
using BycicleShop.Common.SqlContext.Entities;

namespace BycicleShop.Common.SqlContext.Concrete
{
    public class ProductModelDataContext
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

    }
}
