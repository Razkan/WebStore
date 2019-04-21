using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Routing;
using HttpMethodConstraint = System.Web.Http.Routing.HttpMethodConstraint;

namespace WebStore
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute("DefaultApiGet", "Api/{controller}", new {action = "Get"},
                new {httpMethod = new HttpMethodConstraint(HttpMethod.Get)});
            config.Routes.MapHttpRoute("DefaultApiPost", "Api/{controller}", new {action = "Post"},
                new {httpMethod = new HttpMethodConstraint(HttpMethod.Post)});
            config.Routes.MapHttpRoute("DefaultApiPut", "Api/{controller}", new {action = "Put"},
                new {httpMethod = new HttpMethodConstraint(HttpMethod.Put)});
            config.Routes.MapHttpRoute("DefaultApiDelete", "Api/{controller}", new {action = "Delete"},
                new {httpMethod = new HttpMethodConstraint(HttpMethod.Delete)});

            var appXmlType =
                config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t =>
                    t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);
        }
    }
}