using WebStore.Model.Webhooks;

namespace WebStore.Db.Repository
{
    public class WebhookEndpointRepository : Repository<WebhookEndpoint>, IWebhookEndpointRepository
    {
        public WebhookEndpointRepository(IDatabase context) : base(context)
        {
        }
    }
}