using System.Collections.Generic;
using System.Threading.Tasks;

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

        //void Run();

        Task<T> SelectAsync<T>(string id) where T : class;

        Task<T> SelectAsync<T>(params object[] args) where T : class;

        Task<IEnumerable<T>> SelectAllAsync<T>() where T : class;

        Task<bool> ContainsAsync<T>(string id) where T : class;

        Task<bool> ContainsAsync<T>(params object[] args) where T : class;

        Task InsertAsync<T>(T entity) where T : class;

        Task UpdateAsync<T>(T entity) where T : class;

        Task DeleteAsync<T>(T entity) where T : class;
    }

    //public interface IDatabaseAsync
    //{
    //    void Run();

    //    Task<T> SelectAsync<T>(string id) where T : class;

    //    Task<T> SelectAsync<T>(params object[] args) where T : class;

    //    Task<IEnumerable<T>> SelectAllAsync<T>() where T : class;

    //    Task<bool> ContainsAsync<T>(string id) where T : class;

    //    Task<bool> ContainsAsync<T>(params object[] args) where T : class;

    //    Task InsertAsync<T>(T entity) where T : class;

    //    Task UpdateAsync<T>(T entity) where T : class;

    //    Task DeleteAsync<T>(T entity) where T : class;
    //}
}