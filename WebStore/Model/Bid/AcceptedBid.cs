using WebStore.Model.Store;
using WebStore.Model.Users;

namespace WebStore.Model.Bid
{
    public class AcceptedBid : Identifiable
    {
        public AcceptedBid(Biddable bid)
        {
            Bid = bid;
        }

        public StoreProduct StoreProduct => Bid.StoreProduct;
        //public User Buyer => Bid.Buyer;
        public string BidId => Bid.Id;
        public string Id { get; set; }

        private Biddable Bid { get; }
    }
}