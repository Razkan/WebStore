using System.Collections.Generic;
using System.Threading.Tasks;
using WebStore.Model.Product;
using WebStore.Model.Webhooks;

namespace WebStore.Db.Repository
{
    public class WebhookSubscriptionRepository : Repository<WebhookSubscription>, IWebhookSubscriptionRepository
    {
        public WebhookSubscriptionRepository(IDatabase context) : base(context)
        {
        }

        public async Task<IEnumerable<WebhookSubscription>> AllByCategoryAndTypeAsync(Category Category,
            BroadcastType broadcastType) => await Context.SelectAllAsync<WebhookSubscription>(
            $"{nameof(WebhookSubscription.Category)}='{Category.Id}'",
            $"_{broadcastType.ToString()}='True'");
        
    }
}