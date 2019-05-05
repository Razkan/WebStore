using WebStore.Db.Attribute;

namespace WebStore.Model.Product
{
    [Table]
    public class Good : Product
    {
        //public Good(string sellerId)
        //{
        //    SellerId = sellerId;
        //}

        [PrimaryKey]
        public string Id { get; set; }

        //public string SellerId { get; }
        [ForeignKey]
        public Category Category { get; set; }
    }
}