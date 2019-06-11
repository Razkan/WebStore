using WebStore.Db.Attribute;
using WebStore.Model.Accounts;

namespace WebStore.Model.Webhooks
{
    [Table]
    public class Webhook : IWebhook
    {
        [PrimaryKey]
        public string Id { get; private set; }

        [ForeignKey]
        public Account Account { get; private set; }

        [ForeignKey]
        public WebhookEndpoint Endpoint { get; set; }

        public bool Suspended { get; set; }

        public static Webhook Make(Account account, string ipEndpoint, ushort port, string pathEndpoint)
        {
            return new Webhook
            {
                Id = Identification.Generate(),
                Account = account,
                Endpoint = WebhookEndpoint.Make(ipEndpoint, port, pathEndpoint)
            };
        }

        public void Commit()
        {
            throw new System.NotImplementedException();
        }
    }
}