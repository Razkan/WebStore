using System.Threading.Tasks;
using WebStore.Model.Accounts;

namespace WebStore.Db.Repository
{
    public class AccountRepository : Repository<Account>, IAccountRepository
    {
        public AccountRepository(IDatabase context) : base(context)
        {
        }

        public async Task<bool> IsAvailableAsync(string username) =>
            await Context.ContainsAsync<Account>($"{nameof(Account.Username)}='{username}'");
    }
}