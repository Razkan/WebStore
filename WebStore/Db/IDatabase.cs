using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebStore.Db
{
    public interface IDatabase
    {
        void Run();
        
        Task<T> SelectAsync<T>(params object[] args) where T : class;

        Task<IEnumerable<T>> SelectAllAsync<T>() where T : class;

        Task<IEnumerable<T>> SelectAllAsync<T>(params object[] args) where T : class;

        Task<bool> ContainsAsync<T>(params object[] args) where T : class;

        Task InsertAsync<T>(T entity) where T : class;

        Task UpdateAsync<T>(T entity) where T : class;

        Task DeleteAsync<T>(T entity) where T : class;
    }
}