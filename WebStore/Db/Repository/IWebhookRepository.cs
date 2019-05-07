using System.Threading.Tasks;
using WebStore.Model.Accounts;
using DbWebhook = WebStore.Model.Webhooks.Webhook;

namespace WebStore.Db.Repository
{
    public interface IWebhookRepository : IRepository<DbWebhook>
    {
        Task<DbWebhook> ByAccountNotSuspendedAsync(Account account);
    }
}