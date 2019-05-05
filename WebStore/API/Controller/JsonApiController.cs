using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;

namespace WebStore.API
{
    public abstract class JsonApiController<TForm> : ApiController where TForm : class
    {
        public async Task<object> Get() => await HttpGet();

        public async Task<HttpResponseMessage> Post() => await
            HttpPost(JsonConvert.DeserializeObject<TForm>(await Request.Content.ReadAsStringAsync()));

        public async Task<HttpResponseMessage> Put() =>
            await HttpPut(JsonConvert.DeserializeObject<TForm>(await Request.Content.ReadAsStringAsync()));

        public async Task<HttpResponseMessage> Delete() =>
            await HttpDelete(JsonConvert.DeserializeObject<TForm>(await Request.Content.ReadAsStringAsync()));

        protected virtual Task<object> HttpGet() => throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);

        protected virtual Task<HttpResponseMessage> HttpPost(TForm form) =>
            throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);

        protected virtual Task<HttpResponseMessage> HttpPut(TForm form) =>
            throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);

        protected virtual Task<HttpResponseMessage> HttpDelete(TForm form) =>
            throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);
    }

    public abstract class JsonApiController : ApiController
    {
        public async Task<object> Get() => await HttpGet();

        public async Task<HttpResponseMessage> Post() => await
            HttpPost(JsonConvert.DeserializeObject<object>(await Request.Content.ReadAsStringAsync()));

        public async Task<HttpResponseMessage> Put() =>
            await HttpPut(JsonConvert.DeserializeObject<object>(await Request.Content.ReadAsStringAsync()));

        public async Task<HttpResponseMessage> Delete() =>
            await HttpDelete(JsonConvert.DeserializeObject<object>(await Request.Content.ReadAsStringAsync()));

        protected virtual Task<object> HttpGet() => throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);

        protected virtual Task<HttpResponseMessage> HttpPost(object form) =>
            throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);

        protected virtual Task<HttpResponseMessage> HttpPut(object form) =>
            throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);

        protected virtual Task<HttpResponseMessage> HttpDelete(object form) =>
            throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);
    }
}