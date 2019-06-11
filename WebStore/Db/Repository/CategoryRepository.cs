using WebStore.Model.Product;

namespace WebStore.Db.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(IDatabase context) : base(context)
        {
        }
    }
}