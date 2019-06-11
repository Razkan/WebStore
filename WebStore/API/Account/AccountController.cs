using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebStore.Db.UnitOfWorks;
using DbUser = WebStore.Model.Users.User;
using DbAccount = WebStore.Model.Accounts.Account;

namespace WebStore.API
{
    public class AccountController : ApiController
    {
        private UnitOfWork UnitOfWork { get; }

        public AccountController()
        {
            UnitOfWork = new UnitOfWork();
        }

        [Route("api/account/{username}")]
        [HttpGet]
        public async Task<object> CheckAvailability(string username) => new
        {
            available = !await UnitOfWork.AccountRepository.IsAvailableAsync(username)
        };

        [Route("api/account/new")]
        [HttpPost]
        public async Task<HttpResponseMessage> CreateAccount(AccountForm account)
        {
            if (!ModelState.IsValid) return await this.CreateModelErrorResponse(HttpStatusCode.BadRequest);

            if (await UnitOfWork.AccountRepository.ContainsAsync($"{nameof(DbAccount.Username)}='{account.Username}'"))
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Forbidden,
                    $"username '{account.Username}' is not available"));


            // Validate
            //if (string.IsNullOrWhiteSpace(account.Username))
            //    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest,
            //        "missing or invalid username"));
            //if (string.IsNullOrWhiteSpace(account.Password))
            //    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest,
            //        "missing or invalid password"));
            //if (Database.Contains<DbAccount>($"{nameof(DbAccount.Username)}='{account.Username}'"))
            //    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Forbidden,
            //        $"username '{account.Username}' is not available"));


            // Create
            await UnitOfWork.AccountRepository.InsertAsync(DbAccount.Make(account.Username, account.Password,
                DbUser.Make()));

            // Return response
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}