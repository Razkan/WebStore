using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebStore.Db
{
    public interface IDatabase
    {
        void Run();

        //T Select<T>(string id) where T : class;

        T Select<T>(params object[] args) where T : class;

        IEnumerable<T> SelectAll<T>() where T : class;

        //bool Contains<T>(string id) where T : class;

        bool Contains<T>(params object[] args) where T : class;

        void Insert<T>(T entity) where T : class;

        void Update<T>(T entity) where T : class;

        void Delete<T>(T entity) where T : class;

        //Task<T> SelectAsync<T>(string id) where T : class;

        Task<T> SelectAsync<T>(params object[] args) where T : class;

        Task<IEnumerable<T>> SelectAllAsync<T>() where T : class;

        Task<IEnumerable<T>> SelectAllAsync<T>(params object[] args) where T : class;

        //Task<bool> ContainsAsync<T>(string id) where T : class;

        Task<bool> ContainsAsync<T>(params object[] args) where T : class;

        Task InsertAsync<T>(T entity) where T : class;

        Task UpdateAsync<T>(T entity) where T : class;

        Task DeleteAsync<T>(T entity) where T : class;
    }

    //public interface IDatabase<T> : IDatabase where T : class, Identifiable
    //{
    //    T Select(string id);

    //    T Select(params object[] args);

    //    IEnumerable<T> SelectAll();

    //    bool Contains(string id);

    //    bool Contains(params object[] args);

    //    void Insert(T entity);

    //    void Update(T entity);

    //    void Delete(T entity);

    //    Task<T> SelectAsync(string id);

    //    Task<T> SelectAsync(params object[] args);

    //    Task<IEnumerable<T>> SelectAllAsync();

    //    Task<IEnumerable<T>> SelectAllAsync(params object[] args);

    //    Task<bool> ContainsAsync(string id);

    //    Task<bool> ContainsAsync(params object[] args);

    //    Task InsertAsync(T entity);

    //    Task UpdateAsync(T entity);

    //    Task DeleteAsync(T entity);
    //}
}