using System;
using WebStore.Model.Users;
// ReSharper disable UnassignedGetOnlyAutoProperty

namespace WebStore.Model.Store
{
    public class StoreService : StoreProduct
    {
        public double Price { get; set; }

        public Product.Product Product { get; }
        public Store Store { get; }
        public IUser Seller { get; }
        public DateTime CreatedAt { get; }

        public string Id { get; }

        public void Commit()
        {
            throw new NotImplementedException();
        }
    }
}