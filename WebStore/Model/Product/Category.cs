using WebStore.Db.Attribute;

namespace WebStore.Model.Product
{
    [Table]
    public class Category : Identifiable
    {
        [PrimaryKey]
        public string Id { get; private set; }

        [Unique]
        public string Name { get; private set; }

        public static string GetProductCategory<T>(T obj) where T : Product
        {
            return "";
        }
    }
}