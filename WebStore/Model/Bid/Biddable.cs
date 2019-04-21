using WebStore.Model.Store;
using WebStore.Model.Users;

namespace WebStore.Model.Bid
{
    /// <summary>
    /// Used in a timed purchase where only the highest bid wins
    /// </summary>
    public interface Biddable : Identifiable
    {
        StoreProduct StoreProduct { get; }
        IUser Buyer { get; }
        double Price { get; }
    }
}