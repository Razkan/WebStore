using WebStore.Db.Attribute;

namespace WebStore.Model.Webhooks
{
    [Table]
    public class WebhookEndpoint : IDatabaseEntity
    {
        public string Id { get; private set; }

        public string IPAddress { get; set; }

        public ushort Port { get; set; }

        public string Path { get; set; }

        public static WebhookEndpoint Make(string ipEndpoint, ushort port, string pathEndpoint)
        {
            return new WebhookEndpoint
            {
                Id = Identification.Generate(),
                IPAddress = ipEndpoint,
                Port = port,
                Path = pathEndpoint,
            };
        }
    }
}