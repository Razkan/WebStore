using System.Threading.Tasks;
using WebStore.Model.Accounts;

namespace WebStore.Db.Repository
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<bool> IsAvailableAsync(string username);


    }
}