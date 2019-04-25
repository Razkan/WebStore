using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WebStore.Db;
using WebStore.Model.Webhooks;
using DbCategory = WebStore.Model.Product.Category;

namespace WebStore.API
{
    public class WebhookController : JsonApiController<WebhookForm>
    {
        protected override async Task<object> HttpGet()
        {
            var account = await Request.GetAccount();
            var webhook = await Database.SelectAsync<Webhook>($"{nameof(Webhook.Account)}='{account.Id}'");

            if (webhook == null)
            {
                return new
                {
                    Uri = "",
                    Endpoint = new WebhookEndpoint(),
                    Suspended = false,
                    Categories = new string[] { }
                };
            }

            var categories = await Database.SelectAllAsync<WebhookSubscription>(
                $"{nameof(WebhookSubscription.Account)}='{account.Id}'");

            return new
            {
                Uri = $"https://{webhook.Endpoint.IPAddress}:{webhook.Endpoint.Port}/{webhook.Endpoint.Path}",
                webhook.Endpoint,
                Categories = categories.Select(category => new
                {
                    category.Id,
                    Insert = category._Insert,
                    Update = category._Update,
                    Delete = category._Delete
                })
            };
        }

        // api/webhook 
        protected override async Task<HttpResponseMessage> HttpPost(WebhookForm form)
        {
            var account = await Request.GetAccount();
            if (await Database.ContainsAsync<Webhook>($"{nameof(Webhook.Account)}='{account.Id}'"))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "A webhook already exists for this user");
            }

            var endpoint = form.Endpoint;
            await Database.InsertAsync(Webhook.Make(account, endpoint.IPAddress, ushort.Parse(endpoint.Port),
                endpoint.Path));

            foreach (var category in form.Categories)
            {
                await Database.InsertAsync(WebhookSubscription.Make(account,
                    await Database.SelectAsync<DbCategory>(category.Id), category.Insert, category.Update,
                    category.Delete));
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        protected override async Task<HttpResponseMessage> HttpPut(WebhookForm form)
        {
            var account = await Request.GetAccount();
            var hook = await Database.SelectAsync<Webhook>($"{nameof(Webhook.Account)}='{account.Id}'");
            if (hook == null)
            {
                var endpoint = form.Endpoint;
                await Database.InsertAsync(Webhook.Make(account, endpoint.IPAddress, ushort.Parse(endpoint.Port),
                    endpoint.Path));
            }
            else
            {
                var endpoint = form.Endpoint;
                hook.Endpoint.IPAddress = endpoint.IPAddress;
                hook.Endpoint.Port = ushort.Parse(endpoint.Port);
                hook.Endpoint.Path = endpoint.Path;
                await Database.UpdateAsync(hook.Endpoint);
            }

            foreach (var subscription in await Database.SelectAllAsync<WebhookSubscription>(
                $"{nameof(WebhookSubscription.Account)}='{account.Id}'"))
            {
                await Database.DeleteAsync(subscription);
            }

            foreach (var category in form.Categories)
            {
                await Database.InsertAsync(WebhookSubscription.Make(account,
                    await Database.SelectAsync<DbCategory>(category.Id), category.Insert, category.Update,
                    category.Delete));
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        protected override async Task<HttpResponseMessage> HttpDelete(WebhookForm form)
        {
            var account = await Request.GetAccount();
            var hook = await Database.SelectAsync<Webhook>($"{nameof(Webhook.Account)}='{account.Id}'");

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }

    public class WebhookForm
    {
        //public string IPEndpoint { get; set; } // 127.0.0.1
        //public string PortEndpoint { get; set; } // 5000
        //public string PathEndpoint { get; set; } // /webstore_callback
        public WebhookEndpoint Endpoint { get; set; }

        public IEnumerable<Category> Categories { get; set; }

        public class Category
        {
            public string Id { get; set; }
            public bool Insert { get; set; }
            public bool Update { get; set; }
            public bool Delete { get; set; }

            //[JsonProperty("Operations", ItemConverterType = typeof(StringEnumConverter))]
            //public BroadcastType[] Operations { get; set; }
        }

        public class WebhookEndpoint
        {
            public string IPAddress { get; set; } // 127.0.0.1
            public string Port { get; set; } // 5000
            public string Path { get; set; } // /webstore_callback
        }
    }
}