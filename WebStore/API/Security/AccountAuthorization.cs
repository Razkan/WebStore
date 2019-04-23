using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebStore.Db;
using WebStore.Model.Accounts;

namespace WebStore.API.Security
{
    public class AccountAuthorization : IAuthorization
    {
        public async Task Authorize(HttpRequestMessage request)
        {
            var headers = request.Headers;
            if (!headers.TryGetValues("username", out var usernames))
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            if (!headers.TryGetValues("password", out var passwords))
                throw new HttpResponseException(HttpStatusCode.Forbidden);

            var username = usernames.FirstOrDefault();
            var password = passwords.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(username)) throw new HttpResponseException(HttpStatusCode.Forbidden);
            if (string.IsNullOrWhiteSpace(password)) throw new HttpResponseException(HttpStatusCode.Forbidden);

            var account = await Database.SelectAsync<Account>($"{nameof(Account.Username)}='{username}'",
                $"{nameof(Account.Password)}='{password}'");

            if (account == null) throw new HttpResponseException(HttpStatusCode.Forbidden);
        }
    }
}