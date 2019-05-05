using WebStore.Model.Users;

namespace WebStore.Db.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(IDatabase context) : base(context)
        {
        }
    }
}