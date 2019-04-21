using WebStore.Db;

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
    }
}