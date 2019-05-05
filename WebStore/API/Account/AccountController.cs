using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.Results;
using WebStore.Db;
using WebStore.Db.Repository;
using DbUser = WebStore.Model.Users.User;
using DbAccount = WebStore.Model.Accounts.Account;

namespace WebStore.API
{
    public class AccountController : ApiController
    {
        private IAccountRepository AccountRepository { get; }

        public AccountController()
        {
            AccountRepository = new AccountRepository(Database.Instance);
        }

        [Route("api/account/{username}")]
        [HttpGet]
        //[Route("customers/{customerId}/orders")]
        public async Task<object> CheckAvailability(string username) => new
        {
            available = !await AccountRepository.IsAvailableAsync(username)
        };

        [Route("api/account/new")]
        [HttpPost]
        public async Task<HttpResponseMessage> CreateAccount(AccountForm account)
        {
            if (ModelState.IsValid)
            {
                if ( Database.Contains<DbAccount>($"{nameof(DbAccount.Username)}='{account.Username}'") )
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
                await Database.InsertAsync(DbAccount.Make(account.Username, account.Password, DbUser.Make()));

                // Return response
                return Request.CreateResponse(HttpStatusCode.OK);
            }

            return await this.CreateModelErrorResponse(HttpStatusCode.BadRequest);
            //var response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            //return await Task.FromResult(response);

            //return await new ModelStateResult(HttpStatusCode.BadRequest, ModelState, Request).ExecuteAsync();
            //return new ModelStateResult(HttpStatusCode.BadRequest, ModelState, Request);
            //return Request.CreateResponse(HttpStatusCode.BadRequest, );
        }
    }

    public class ModelStateResult : IHttpActionResult
    {
        public HttpStatusCode Status { get; }
        public ModelStateDictionary ModelState { get; }
        public HttpRequestMessage Request { get; }

        public ModelStateResult(HttpStatusCode status, ModelStateDictionary modelState, HttpRequestMessage request)
        {
            Status = status;
            ModelState = modelState;
            Request = request;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = Request.CreateErrorResponse(Status, ModelState);
            return Task.FromResult(response);
        }

        public Task<HttpResponseMessage> ExecuteAsync() => ExecuteAsync(new CancellationToken());
    }


    //public class AccountController : JsonApiController<AccountForm>
    //{
    //    //protected override IEnumerable<AccountForm> HttpGet() => new[] {new AccountForm {Name = "abc"}};
    //    [Route("api/account/{username}")]
    //    public async Task<object> Get(string username) => new
    //        {available = !await Database.ContainsAsync<Account>($"{nameof(Account.Username)}='{username}'")};

    //    protected override async Task<HttpResponseMessage> HttpPost(AccountForm form)
    //    {
    //        if (string.IsNullOrWhiteSpace(form.Username))
    //            throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest,
    //                "missing or invalid username"));
    //        if (string.IsNullOrWhiteSpace(form.Password))
    //            throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest,
    //                "missing or invalid password"));
    //        if (Database.Contains<Account>($"{nameof(Account.Username)}='{form.Username}'"))
    //            throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Forbidden,
    //                $"username '{form.Username}' is not available"));

    //        await Database.InsertAsync(Account.Make(form.Username, form.Password, WSUser.Make()));

    //        return Request.CreateResponse(HttpStatusCode.OK);
    //    }
    //}
}