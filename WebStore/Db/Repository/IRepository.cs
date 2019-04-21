using System.Collections.Generic;
using WebStore.Model.Users;

namespace WebStore.Db.Repository
{
    public interface IRepository<T> where T : class
    {
        T Select(string id);

        IEnumerable<T> SelectAll();

        void Insert(T entity);

        void Update(T entity);

        void Delete(T entity);
    }

    public interface IUserRepository : IRepository<User>
    {
    }

    public interface ISystemUserRepository : IRepository<SystemUser>
    {
    }

    public class Repository<T> : IRepository<T> where T : class
    {
        private IDatabase Entities { get; }

        protected Repository(IDatabase context)
        {
            Entities = context;
        }

        public T Select(string id) => Entities.Select<T>(id);

        public IEnumerable<T> SelectAll() => Entities.SelectAll<T>();

        public void Insert(T entity) => Entities.Insert(entity);

        public void Update(T entity) => Entities.Update(entity);

        public void Delete(T entity) => Entities.Delete(entity);
    }

    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(IDatabase context) : base(context)
        {
        }
    }

    public class SystemUserRepository : Repository<SystemUser>, ISystemUserRepository
    {
        public SystemUserRepository(IDatabase context) : base(context)
        {
        }
    }
}