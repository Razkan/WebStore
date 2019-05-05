//using System.Net;
//using System.Net.Http;
//using System.Threading.Tasks;
//using System.Web.Http;
//using System.Web.UI.WebControls;
//using WebStore.Db;
//using WebStore.Db.Repository;
//using DbAccount = WebStore.Model.Accounts.Account;

//namespace WebStore.API.Account
//{
//    public class AccountValidation : IValidatable<AccountForm>
//    {
//        IRepository<DbAccount> AccountRepository { get; }

//        public AccountValidation() => AccountRepository = new AccountRepository(Database.Instance);

//        public async Task ValidateAsync(AccountForm form)
//        {
//            "Abc".ToHttpException(HttpStatusCode.BadRequest);

//            throw new HttpResponseException(new HttpResponseMessage
//                {StatusCode = HttpStatusCode.BadRequest, Content = new StringContent("abc")});

//            if ( string.IsNullOrWhiteSpace(form.Username) )
//                throw new HttpResponseException("Missing or invalid username");
//            if ( string.IsNullOrWhiteSpace(form.Password) )
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest,
//                    "missing or invalid password"));
//            if ( Database.Contains<DbAccount>($"{nameof(DbAccount.Username)}='{form.Username}'") )
//                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Forbidden,
//                    $"username '{form.Username}' is not available"));


//            return true;
//        }
//    }
//}