using System;
using System.Collections.Concurrent;

namespace WebStore.Model.Store
{
    /// <inheritdoc />
    /// <summary>
    /// The container for all the biddable and purchasable products
    /// </summary>
    public class Store : IDatabaseEntity
    {
        public Store(String id)
        {
            Id = id;
            StoreProducts = new ConcurrentDictionary<string, StoreProduct>();
        }

        private ConcurrentDictionary<string, StoreProduct> StoreProducts { get; }

        public bool Add(StoreProduct storeProduct) => StoreProducts.TryAdd(storeProduct.Id, storeProduct);

        public bool Remove(string id) => StoreProducts.TryRemove(id, out _);

        public bool Contains(string id) => StoreProducts.ContainsKey(id);

        public int Count() => StoreProducts.Count;

        public string Id { get; }

        public void Commit()
        {
            throw new NotImplementedException();
        }
    }
}