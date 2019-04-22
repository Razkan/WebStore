using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebStore.Db;
using WebStore.Model.Accounts;
using WSUser = WebStore.Model.Users.User;

namespace WebStore.API
{
    public class AccountController : JsonApiController<AccountForm>
    {
        //protected override IEnumerable<AccountForm> HttpGet() => new[] {new AccountForm {Name = "abc"}};
        [Route("api/account/{username}")]
        public async Task<object> Get(string username) => new
            {available = !await Database.ContainsAsync<Account>($"{nameof(Account.Username)}='{username}'")};

        protected override async Task HttpPost(AccountForm form)
        {
            if (string.IsNullOrWhiteSpace(form.Username))
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                    "missing or invalid username"));
            if (string.IsNullOrWhiteSpace(form.Password))
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                    "missing or invalid password"));
            if (Database.Contains<Account>($"{nameof(Account.Username)}='{form.Username}'"))
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Forbidden,
                    $"username '{form.Username}' is not available"));

            await Database.InsertAsync(Account.Make(form.Username, form.Password, WSUser.Make()));

            Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}