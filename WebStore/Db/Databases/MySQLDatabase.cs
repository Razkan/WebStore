using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebStore.Db
{
    public class MySQLDatabase : IDatabase
    {
        private IDatabaseConfiguration Configuration { get; }

        public MySQLDatabase(IDatabaseConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void Run()
        {
            throw new NotImplementedException();
        }

        public Task<T> SelectAsync<T>(params object[] args) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> SelectAllAsync<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> SelectAllAsync<T>(params object[] args) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<bool> ContainsAsync<T>(params object[] args) where T : class
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync<T>(T entity) where T : class
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync<T>(T entity) where T : class
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync<T>(T entity) where T : class
        {
            throw new NotImplementedException();
        }

        public static IDatabase Make(IDatabaseConfiguration configuration) => new MySQLDatabase(configuration);
    }
}