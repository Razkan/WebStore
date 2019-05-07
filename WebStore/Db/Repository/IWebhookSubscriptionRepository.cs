using System.Collections.Generic;
using System.Threading.Tasks;
using WebStore.Model.Product;
using WebStore.Model.Webhooks;
using DbWebhookSubscription = WebStore.Model.Webhooks.WebhookSubscription;

namespace WebStore.Db.Repository
{
    public interface IWebhookSubscriptionRepository : IRepository<DbWebhookSubscription>
    {
        Task<IEnumerable<DbWebhookSubscription>> AllByCategoryAndBroadcastTypeAsync(Category Category,
            BroadcastType broadcastType);
    }
}