using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebStore.Db;
using WebStore.Model.Accounts;

namespace WebStore.API
{
    public static class ApiExtensions
    {
        public static async Task<Account> GetAccount(this HttpRequestMessage request)
        {
            var headers = request.Headers;
            headers.TryGetValues("username", out var usernames);
            headers.TryGetValues("password", out var passwords);

            var username = usernames.FirstOrDefault();
            var password = passwords.FirstOrDefault();

            return await Database.SelectAsync<Account>($"{nameof(Account.Username)}='{username}'",
                $"{nameof(Account.Password)}='{password}'");
        }
    }
}