using System.Threading.Tasks;
using WebStore.Model.Accounts;
using DbWebhook = WebStore.Model.Webhooks.Webhook;

namespace WebStore.Db.Repository
{
    public class WebhookRepository : Repository<DbWebhook>, IWebhookRepository
    {
        public WebhookRepository(IDatabase context) : base(context)
        {
        }

        public async Task<DbWebhook> ByAccountNotSuspendedAsync(Account account) => await Database.SelectAsync<DbWebhook>(
            $"{nameof(DbWebhook.Account)}='{account.Id}'",
            $"{nameof(DbWebhook.Suspended)}='False'");
    }
}