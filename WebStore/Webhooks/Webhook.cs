using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WebStore.Db;
using WebStore.Model.Product;
using WebStore.Model.Webhooks;
using DbWebhook = WebStore.Model.Webhooks.Webhook;
using Task = System.Threading.Tasks.Task;

namespace WebStore.Webhooks
{
    public sealed class Webhook
    {
        private static HttpClient Client { get; }

        static Webhook()
        {
            Client = new HttpClient();
        }

        public static async Task Broadcast<T>(T obj, BroadcastType type) where T : Product
        {
            var connections = (await Task.WhenAll((await Database.SelectAllAsync<WebhookSubscription>(
                        $"{nameof(WebhookSubscription.Category)}='{Category.GetProductCategory(obj)}'",
                        $"_{type.ToString()}='True'"))
                    .Select(async subscription => await Database.SelectAsync<DbWebhook>(
                        $"{nameof(DbWebhook.Account)}='{subscription.Account}'",
                        $"{nameof(DbWebhook.Suspended)}='False'"))))
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
                var endpoint = Webhook.Endpoint;
                var uri = new Uri($"https://{endpoint.IPAddress}:{endpoint.Port}/{endpoint.Path}");
                var content = new StringContent(JsonConvert.SerializeObject(new {Type = BroadcastType, Entity = Obj}),
                    Encoding.UTF8, "application/json");

                while (!(await SendAsync(uri, content)).IsSuccessStatusCode && Retry++ < MaxRetries)
                    await Task.Delay(TimeSpan.FromSeconds(RetryDelay));
            }

            private static async Task<HttpResponseMessage> SendAsync(Uri uri, HttpContent content) =>
                await Client.PostAsync(uri, content);
        }
    }
}