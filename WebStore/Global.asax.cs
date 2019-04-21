using System.Web;
using System.Web.Http;
using WebStore.Db;

namespace WebStore
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            Configuration.Register();
            Database.Register();
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}