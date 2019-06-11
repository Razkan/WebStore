using System.Collections.Generic;
using System.Threading.Tasks;
using WebStore.Model;

namespace WebStore.Db.Repository
{
    public class Repository<T> : IRepository<T> where T : class, IDatabaseEntity
    {
        protected IDatabase Context { get; }

        protected Repository(IDatabase context)
        {
            Context = context;
        }

        public async Task<T> SelectAsync(params object[] args) => await Context.SelectAsync<T>(args);

        public async Task<IEnumerable<T>> SelectAllAsync() => await Context.SelectAllAsync<T>();

        public async Task<bool> ContainsAsync(params object[] args) => await Context.ContainsAsync<T>(args);

        public async Task<IEnumerable<T>> SelectAllAsync(params object[] args) => await Context.SelectAllAsync<T>(args);

        public async Task InsertAsync(T entity) => await Context.InsertAsync(entity);

        public async Task UpdateAsync(T entity) => await Context.UpdateAsync(entity);

        public async Task DeleteAsync(T entity) => await Context.DeleteAsync(entity);
    }
}