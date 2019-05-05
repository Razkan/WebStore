using WebStore.Db.Attribute;

namespace WebStore.Model.Product
{
    class Task : Product
    {
        [PrimaryKey]
        public string Id { get; set; }

        [ForeignKey]
        public Category Category { get; set; }
    }
}