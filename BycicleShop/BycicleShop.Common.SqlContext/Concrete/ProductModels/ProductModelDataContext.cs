using System.Data.Entity;
using System.Linq;
using BycicleShop.Common.SqlContext.Entities;

namespace BycicleShop.Common.SqlContext.Concrete
{
    public class ProductModelDataContext
    {
        private AdventureWorks _dataContext = new AdventureWorks();

        public DbSet<ProductModel> Entity
        {
            get { return _dataContext.ProductModels; }
        }

        private ProductModel Get(int id)
        {
            return Entity.First(x => x.ProductModelID == id);
        }

        public ProductModel Get(string name)
        {
            return Entity.First(x => x.Name == name);
        }

    }
}
