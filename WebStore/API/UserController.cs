using System.Web.Http;

namespace WebStore.API
{
    public class UserController : ApiController
    {
        public void Post([FromBody] string account, string pass)
        {

        }
    }
}
