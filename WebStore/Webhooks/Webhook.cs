using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WebStore.Db;
using WebStore.Model.Webhooks;
using DbWebhook = WebStore.Model.Webhooks.Webhook;

namespace WebStore.Webhooks
{
    public sealed class Webhook
    {
        private static HttpClient Client { get; }

        static Webhook()
        {
            Client = new HttpClient();
        }

        public static async Task Broadcast<T>(T obj, BroadcastType type) where T : class
        {
            var connections = (await Database.SelectAllAsync<DbWebhook>())
                .Where(e => !e.Suspended)
                .Select(hook => new WebhookConnection(hook, type, obj))
                .ToList();

            var tasks = connections.Select(connection => Task.Run(connection.Start)).ToArray();
            Task.WaitAll(tasks);

            foreach (var connection in connections)
            {
                if (connection.Failed)
                {
                    // TODO suspend webhook connection, send email etc
                }
            }
        }

        private class WebhookConnection
        {
            private const int MaxRetries = 3;
            private const int RetryDelay = 30;

            private IWebhook Webhook { get; }

            [JsonConverter(typeof(StringEnumConverter))]
            private BroadcastType BroadcastType { get; }

            private object Obj { get; }
            private int Retry { get; set; }

            public bool Failed => Retry == 3;

            public WebhookConnection(IWebhook webhook, BroadcastType broadcastType, object obj)
            {
                Webhook = webhook;
                BroadcastType = broadcastType;
                Obj = obj;
            }

            public async Task Start()
            {
                var uri = new Uri($"https://{Webhook.IPEndpoint}:{Webhook.PortEndpoint}/{Webhook.PathEndpoint}");
                var content = new StringContent(JsonConvert.SerializeObject(new {Type = BroadcastType, Object = Obj}),
                    Encoding.UTF8, "application/json");

                while (!(await SendAsync(uri, content)).IsSuccessStatusCode && Retry++ < MaxRetries)
                    await Task.Delay(TimeSpan.FromSeconds(RetryDelay));
            }

            private static async Task<HttpResponseMessage> SendAsync(Uri uri, HttpContent content) =>
                await Client.PostAsync(uri, content);
        }
    }
}