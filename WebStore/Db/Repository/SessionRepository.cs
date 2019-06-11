using WebStore.Model.Session;

namespace WebStore.Db.Repository
{
    public class SessionRepository : Repository<Session>, ISessionRepository
    {
        public SessionRepository(IDatabase context) : base(context)
        {
        }
    }
}