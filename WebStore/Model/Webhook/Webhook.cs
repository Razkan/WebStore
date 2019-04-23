using Newtonsoft.Json;
using WebStore.API;
using WebStore.Db.Attribute;
using WebStore.Model.Accounts;

namespace WebStore.Model.Webhooks
{
    public interface IWebhook : Identifiable
    {
        string IPEndpoint { get; set; }
        ushort PortEndpoint { get; set; }
        string PathEndpoint { get; set; }
    }

    [Table]
    public class Webhook : IWebhook
    {
        [PrimaryKey]
        public string Id { get; private set; }

        [ForeignKey]
        public Account Account { get; private set; }

        public string IPEndpoint { get; set; }

        public ushort PortEndpoint { get; set; }

        public string PathEndpoint { get; set; }

        public bool Suspended { get; set; }

        public static Webhook Make(Account account, string ipEndpoint, ushort port, string pathEndpoint) =>
            new Webhook
            {
                Id = Identification.Generate(),
                Account = account,
                IPEndpoint = ipEndpoint,
                PortEndpoint = port,
                PathEndpoint = pathEndpoint,
            };
    }

    [Table]
    public class WebhookCategory
    {
        public string Id { get; private set; }

        [ForeignKey]
        public Webhook Webhook { get; private set; }

        public bool _Insert { get; set; }
        public bool _Update { get; set; }
        public bool _Delete { get; set; }

        public static WebhookCategory Make(string id, Webhook webhook, bool insert, bool update, bool delete)
        {
            return new WebhookCategory
            {
                Id = id,
                Webhook = webhook,
                _Insert = insert,
                _Update = update,
                _Delete = delete
            };
        }
    }
}