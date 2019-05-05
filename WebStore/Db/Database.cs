using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebStore.Db
{
    public static class Database
    {
        internal static IDatabase Instance { get; set; }

        public static void Register()
        {
            Instance = new SQLiteDatabase();
            Instance.Run();

            //var good = new Good { Id = "abc" };
            //Instance.Insert(good);

            //var good = Select<Good>("abc");
            //var stopwatch = new Stopwatch();
            //Instance.Insert(
            //new Account("kalle", "123", new User {Id = "dd", FirstName = "Frank", LastName = "Larsson"}));
            //var user = Instance.Select<User>("dd");
        }

        public static T Select<T>(params object[] args) where T : class => Instance.Select<T>(args: args);

        public static async Task<T> SelectAsync<T>(params object[] args) where T : class =>
            await Instance.SelectAsync<T>(args: args);

        public static IEnumerable<T> SelectAll<T>() where T : class => Instance.SelectAll<T>();

        public static async Task<IEnumerable<T>> SelectAllAsync<T>() where T : class =>
            await Instance.SelectAllAsync<T>();

        public static bool Contains<T>(params object[] args) where T : class => Instance.Contains<T>(args: args);

        public static async Task<bool> ContainsAsync<T>(params object[] args) where T : class =>
            await Instance.ContainsAsync<T>(args: args);

        public static async Task<IEnumerable<T>> SelectAllAsync<T>(params object[] args) where T : class =>
            await Instance.SelectAllAsync<T>(args);

        public static void Insert<T>(T entity) where T : class => Instance.Insert(entity);

        public static async Task InsertAsync<T>(T entity) where T : class => await Instance.InsertAsync(entity);

        public static void Update<T>(T entity) where T : class => Instance.Update(entity);

        public static async Task UpdateAsync<T>(T entity) where T : class => await Instance.UpdateAsync(entity);

        public static void Delete<T>(T entity) where T : class => Instance.Delete(entity);

        public static async Task DeleteAsync<T>(T entity) where T : class => await Instance.DeleteAsync(entity);
    }
}