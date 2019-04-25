﻿using System;
using WebStore.Model.Users;
// ReSharper disable UnassignedGetOnlyAutoProperty

namespace WebStore.Model.Store
{
    public class StoreGood : StoreProduct
    {
        public int Quantity { get; set; }
        public double Price { get; set; }

        public Product.Product Product { get; }
        public Store Store { get; }
        public IUser Seller { get; }
        public DateTime CreatedAt { get; }

        public string Id { get; }
    }
}