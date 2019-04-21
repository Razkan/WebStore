using System.Collections.Generic;

namespace WebStore.Db
{
    public interface IDatabase
    {
        void Run();

        T Select<T>(string id) where T : class;

        T Select<T>(params object[] args) where T : class;

        IEnumerable<T> SelectAll<T>() where T : class;

        bool Contains<T>(string id) where T : class;

        bool Contains<T>(params object[] args) where T : class;

        void Insert<T>(T entity) where T : class;

        void Update<T>(T entity) where T : class;

        void Delete<T>(T entity) where T : class;
    }
}