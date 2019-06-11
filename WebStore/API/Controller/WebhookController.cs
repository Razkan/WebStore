using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WebStore.Db.UnitOfWorks;
using WebStore.Model.Webhooks;
using DbCategory = WebStore.Model.Product.Category;

namespace WebStore.API
{
    public class WebhookController : JsonApiController<WebhookForm>
    {
        private UnitOfWork UnitOfWork { get; }

        public WebhookController()
        {
            UnitOfWork = new UnitOfWork();
        }

        protected override async Task<object> HttpGet()
        {
            var account = await Request.GetAccount();
            var webhook = await UnitOfWork.WebhookRepository.SelectAsync($"{nameof(Webhook.Account)}='{account.Id}'");

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

            var categories =
                await UnitOfWork.WebhookSubscriptionRepository.SelectAllAsync(
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
            if (await UnitOfWork.WebhookRepository.ContainsAsync($"{nameof(Webhook.Account)}='{account.Id}'"))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "A webhook already exists for this user");
            }

            var endpoint = form.Endpoint;
            await UnitOfWork.WebhookRepository.InsertAsync(Webhook.Make(account, endpoint.IPAddress,
                ushort.Parse(endpoint.Port),
                endpoint.Path));

            foreach (var category in form.Categories)
            {
                await UnitOfWork.WebhookSubscriptionRepository.InsertAsync(WebhookSubscription.Make(account,
                    await UnitOfWork.CategoryRepository.SelectAsync(category.Id), category.Insert, category.Update,
                    category.Delete));
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        protected override async Task<HttpResponseMessage> HttpPut(WebhookForm form)
        {
            var account = await Request.GetAccount();
            var hook = await UnitOfWork.WebhookRepository.SelectAsync($"{nameof(Webhook.Account)}='{account.Id}'");
            if (hook == null)
            {
                var endpoint = form.Endpoint;
                await UnitOfWork.WebhookRepository.InsertAsync(Webhook.Make(account, endpoint.IPAddress,
                    ushort.Parse(endpoint.Port),
                    endpoint.Path));
            }
            else
            {
                var endpoint = form.Endpoint;
                hook.Endpoint.IPAddress = endpoint.IPAddress;
                hook.Endpoint.Port = ushort.Parse(endpoint.Port);
                hook.Endpoint.Path = endpoint.Path;
                await UnitOfWork.WebhookEndpointRepository.UpdateAsync(hook.Endpoint);
            }

            foreach (var subscription in await UnitOfWork.WebhookSubscriptionRepository.SelectAllAsync(
                $"{nameof(WebhookSubscription.Account)}='{account.Id}'"))
            {
                await UnitOfWork.WebhookSubscriptionRepository.DeleteAsync(subscription);
            }

            foreach (var category in form.Categories)
            {
                await UnitOfWork.WebhookSubscriptionRepository.InsertAsync(WebhookSubscription.Make(account,
                    await UnitOfWork.CategoryRepository.SelectAsync(category.Id), category.Insert, category.Update,
                    category.Delete));
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

#pragma warning disable 1998
        protected override async Task<HttpResponseMessage> HttpDelete(WebhookForm form)
#pragma warning restore 1998
        {
            // TODO Add delete for a wehbook
            throw new NotImplementedException();

            //var account = await Request.GetAccount();
            //var hook = await Database.SelectAsync<Webhook>($"{nameof(Webhook.Account)}='{account.Id}'");

            //return Request.CreateResponse(HttpStatusCode.OK);
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