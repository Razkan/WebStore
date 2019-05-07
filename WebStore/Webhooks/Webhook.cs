using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using WebStore.Db;
using WebStore.Db.Repository;
using WebStore.Model.Product;
using WebStore.Model.Webhooks;
using Task = System.Threading.Tasks.Task;

namespace WebStore.Webhooks
{
    public static class Webhook
    {
        private static IWebhookRepository WebhookRepository { get; }
        private static IWebhookSubscriptionRepository WebhookSubscriptionRepository { get; }

        static Webhook()
        {
            WebhookRepository = new WebhookRepository(Database.Instance);
            WebhookSubscriptionRepository = new WebhookSubscriptionRepository(Database.Instance);
        }

        public static async Task Broadcast<T>(T obj, BroadcastType broadcastType) where T : Product
        {
            var connections = (await Task
                    .WhenAll((await WebhookSubscriptionRepository.AllByCategoryAndBroadcastTypeAsync(obj.Category,
                        broadcastType)).Select(async subscription =>
                        await WebhookRepository.ByAccountNotSuspendedAsync(subscription.Account))))
                .Select(hook => new WebhookConnection(hook, broadcastType, obj))
                .ToList();

            var tasks = connections.Select(connection => Task.Run(connection.Start)).ToArray();
            await Task.WhenAll(tasks);

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
            static WebhookConnection()
            {
                Client = new HttpClient();
            }

            private static HttpClient Client { get; }

            private const int MaxRetries = 3;
            private const int RetryDelay = 30;

            private IWebhook Webhook { get; }

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
                var uri = new Uri(
                    $"https://{Webhook.Endpoint.IPAddress}:{Webhook.Endpoint.Port}/{Webhook.Endpoint.Path}");
                var content = new StringContent(
                    JsonConvert.SerializeObject(new {Type = BroadcastType.ToString(), Entity = Obj}),
                    Encoding.UTF8, "application/json");

                while (!(await Client.PostAsync(uri, content)).IsSuccessStatusCode && Retry++ < MaxRetries)
                    await Task.Delay(TimeSpan.FromSeconds(RetryDelay));
            }
        }
    }
}