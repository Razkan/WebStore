using System;
using WebStore.Model.Users;

namespace WebStore.Model.Store
{
    public interface StoreProduct : Identifiable
    {
        double Price { get; set; }

        Product.Product Product { get; }
        IUser Seller { get; }
        Store Store { get; }
        DateTime CreatedAt { get; }
    }
}