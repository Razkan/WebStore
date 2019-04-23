using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;

namespace WebStore.API.Security
{
    public abstract class AuthorizationJsonApiController<TForm> : ApiController where TForm : class
    {
        private IAuthorization Authorization { get; }

        protected AuthorizationJsonApiController(IAuthorization authorization)
        {
            Authorization = authorization;
        }

        public async Task<object> Get()
        {
            await Authorization.Authorize(Request);
            return await HttpGet();
        }

        public async Task Post()
        {
            await Authorization.Authorize(Request);
            await HttpPost(JsonConvert.DeserializeObject<TForm>(await Request.Content.ReadAsStringAsync()));
        }

        public async Task Put()
        {
            await Authorization.Authorize(Request);
            await HttpPut(JsonConvert.DeserializeObject<TForm>(await Request.Content.ReadAsStringAsync()));
        }

        public async Task Delete()
        {
            await Authorization.Authorize(Request);
            await HttpDelete(JsonConvert.DeserializeObject<TForm>(await Request.Content.ReadAsStringAsync()));
        }

        protected virtual Task<object> HttpGet() => throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);

        protected virtual Task HttpPost(TForm form) => throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);

        protected virtual Task HttpPut(TForm form) => throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);

        protected virtual Task HttpDelete(TForm form) =>
            throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);
    }

    public abstract class AuthorizationJsonApiController : ApiController
    {
        private IAuthorization Authorization { get; }

        protected AuthorizationJsonApiController(IAuthorization authorization)
        {
            Authorization = authorization;
        }

        public async Task<object> Get()
        {
            await Authorization.Authorize(Request);
            return await HttpGet();
        }

        public async Task Post()
        {
            await Authorization.Authorize(Request);
            await HttpPost(JsonConvert.DeserializeObject<object>(await Request.Content.ReadAsStringAsync()));
        }

        public async Task Put()
        {
            await Authorization.Authorize(Request);
            await HttpPut(JsonConvert.DeserializeObject<object>(await Request.Content.ReadAsStringAsync()));
        }

        public async Task Delete()
        {
            await Authorization.Authorize(Request);
            await HttpDelete(JsonConvert.DeserializeObject<object>(await Request.Content.ReadAsStringAsync()));
        }

        protected virtual Task<object> HttpGet() => throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);

        protected virtual Task HttpPost(object form) =>
            throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);

        protected virtual Task HttpPut(object form) => throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);

        protected virtual Task HttpDelete(object form) =>
            throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);
    }
}