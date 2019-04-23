using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WebStore.Db;
using WebStore.Model.Webhooks;

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
                    IPEndpoint = "",
                    PortEndpoint = (ushort) 0,
                    PathEndpoint = "",
                    Suspended = false,
                    Categories = new string[] { }
                };
            }

            var categories =
                await Database.SelectAllAsync<WebhookCategory>($"{nameof(WebhookCategory.Webhook)}='{webhook.Id}'");

            return new
            {
                Uri = $"https://{webhook.IPEndpoint}:{webhook.PortEndpoint}/{webhook.PathEndpoint}",
                webhook.IPEndpoint,
                webhook.PortEndpoint,
                webhook.PathEndpoint,
                webhook.Suspended,
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

            var webhook = Webhook.Make(account, form.IPEndpoint, ushort.Parse(form.PortEndpoint),
                form.PathEndpoint);
            await Database.InsertAsync(webhook);

            foreach (var category in form.Categories)
            {
                var operations = category.Operations.ToList();
                await Database.InsertAsync(WebhookCategory.Make(category.Id, webhook, operations.Contains(BroadcastType.Insert),
                    operations.Contains(BroadcastType.Update), operations.Contains(BroadcastType.Delete)));
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        protected override async Task<HttpResponseMessage> HttpPut(WebhookForm form)
        {
            var account = await Request.GetAccount();
            var hook = await Database.SelectAsync<Webhook>($"{nameof(Webhook.Account)}='{account.Id}'");
            if (hook == null)
            {
                await Database.InsertAsync(Webhook.Make(account, form.IPEndpoint, ushort.Parse(form.PortEndpoint),
                    form.PathEndpoint));
            }
            else
            {
                hook.IPEndpoint = form.IPEndpoint;
                hook.PortEndpoint = ushort.Parse(form.PortEndpoint);
                hook.PathEndpoint = form.PathEndpoint;
                await Database.UpdateAsync(hook);
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
        public string IPEndpoint { get; set; } // 127.0.0.1
        public string PortEndpoint { get; set; } // 5000
        public string PathEndpoint { get; set; } // /webstore_callback

        public IEnumerable<Category> Categories { get; set; }

        public class Category
        {
            public string Id { get; set; }

            [JsonProperty("Operations", ItemConverterType = typeof(StringEnumConverter))]
            public BroadcastType[] Operations { get; set; }
        }
    }
}