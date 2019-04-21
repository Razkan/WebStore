using WebStore.Model.Store;
using WebStore.Model.Users;

namespace WebStore.Model.Bid
{
    public class AuctionBid : Biddable
    {
        public AuctionBid(StoreProduct sp, IUser buyer, double price)
        {
            StoreProduct = sp;
            Buyer = buyer;
            Price = price;
        }

        public double Price { get; }
        public StoreProduct StoreProduct { get; }
        public IUser Buyer { get; }

        public string Id { get; set; }
    }
}