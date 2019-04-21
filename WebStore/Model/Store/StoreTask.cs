using System;
using WebStore.Model.Users;

namespace WebStore.Model.Store
{
    public class StoreTask : StoreProduct
    {
        public double Price { get; set; }

        public Product.Product Product { get; }
        public Store Store { get; }
        public IUser Seller { get; }
        public DateTime CreatedAt { get; }

        public string Id { get; }
    }
}