using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebStore.Db
{
    // TODO Refactor how the task handling first completion is done. Check https://codeblog.jonskeet.uk/2012/01/16/eduasync-part-19-ordering-by-completion-ahead-of-time/ and https://stackoverflow.com/questions/37528738/is-there-default-way-to-get-first-task-that-finished-successfully

    public class MultiDatabaseHandler : IDatabase
    {
        private IEnumerable<IDatabase> Databases { get; }

        public MultiDatabaseHandler(IEnumerable<IDatabase> databases)
        {
            Databases = databases;
        }

        public void Run() => Databases.ForEach(db => db.Run());

        public async Task<T> SelectAsync<T>(params object[] args) where T : class
        {
            var tasks = Databases.Select(db => Task.Run(() => db.SelectAsync<T>(args)));
            return await await Task.WhenAny(tasks);
        }

        public async Task<IEnumerable<T>> SelectAllAsync<T>() where T : class
        {
            var tasks = Databases.Select(db => Task.Run(db.SelectAllAsync<T>));
            return await await Task.WhenAny(tasks);
        }

        public async Task<IEnumerable<T>> SelectAllAsync<T>(params object[] args) where T : class
        {
            var tasks = Databases.Select(db => Task.Run(() => db.SelectAllAsync<T>(args)));
            return await await Task.WhenAny(tasks);
        }

        public async Task<bool> ContainsAsync<T>(params object[] args) where T : class
        {
            var tasks = Databases.Select(db => Task.Run(() => db.ContainsAsync<T>(args)));
            return await await Task.WhenAny(tasks);
        }

        public async Task InsertAsync<T>(T entity) where T : class
        {
            var tasks = Databases.Select(db => Task.Run(() => db.InsertAsync(entity)));
            await Task.WhenAll(tasks);
        }

        public async Task UpdateAsync<T>(T entity) where T : class
        {
            var tasks = Databases.Select(db => Task.Run(() => db.UpdateAsync(entity)));
            await Task.WhenAll(tasks);
        }

        public async Task DeleteAsync<T>(T entity) where T : class
        {
            var tasks = Databases.Select(db => Task.Run(() => db.DeleteAsync(entity)));
            await Task.WhenAll(tasks);
        }
    }
}