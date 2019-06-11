using System.Collections.Generic;
using System.Threading.Tasks;
using WebStore.Model;

namespace WebStore.Db.Repository
{
    // TODO change sealed database class to repositories, can add specific queries within each
    public interface IRepository<T> where T : class, IDatabaseEntity
    {
        //T Select(string id);

        //IEnumerable<T> SelectAll();

        //void Insert(T entity);

        //void Update(T entity);

        //void Delete(T entity);

        Task<T> SelectAsync(params object[] args);

        Task<IEnumerable<T>> SelectAllAsync();

        Task<bool> ContainsAsync(params object[] args);

        Task<IEnumerable<T>> SelectAllAsync(params object[] args);

        Task InsertAsync(T entity);

        Task UpdateAsync(T entity);

        Task DeleteAsync(T entity);
    }
}