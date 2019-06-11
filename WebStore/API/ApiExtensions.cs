using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebStore.Db.UnitOfWorks;
using DbAccount = WebStore.Model.Accounts.Account;

namespace WebStore.API
{
    public static class ApiExtensions
    {
        private static UnitOfWork UnitOfWork { get; }

        static ApiExtensions()
        {
            UnitOfWork = new UnitOfWork();
        }

        public static async Task<DbAccount> GetAccount(this HttpRequestMessage request)
        {
            var headers = request.Headers;
            headers.TryGetValues("username", out var usernames);
            headers.TryGetValues("password", out var passwords);

            var username = usernames.FirstOrDefault();
            var password = passwords.FirstOrDefault();

            return await UnitOfWork.AccountRepository.SelectAsync($"{nameof(DbAccount.Username)}='{username}'",
                $"{nameof(DbAccount.Password)}='{password}'");
        }

        //public static async Task<TForm> ReadFromJson<TForm>(this HttpRequestMessage request) =>
        //    JsonConvert.DeserializeObject<TForm>(await request.Content.ReadAsStringAsync());

        public static async Task<HttpResponseMessage> CreateModelErrorResponse(this ApiController controller,
            HttpStatusCode code)
        {
            var response = controller.Request.CreateErrorResponse(code, controller.ModelState);
            return await Task.FromResult(response);
        }
    }
}