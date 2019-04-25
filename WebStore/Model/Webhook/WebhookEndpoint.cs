using WebStore.Db.Attribute;

namespace WebStore.Model.Webhooks
{
    [Table]
    public class WebhookEndpoint
    {
        public string IPAddress { get; set; }

        public ushort Port { get; set; }

        public string Path { get; set; }

        public static WebhookEndpoint Make(string ipEndpoint, ushort port, string pathEndpoint)
        {
            return new WebhookEndpoint
            {
                IPAddress = ipEndpoint,
                Port = port,
                Path = pathEndpoint,
            };
        }
    }
}