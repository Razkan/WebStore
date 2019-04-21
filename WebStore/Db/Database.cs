using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace WebStore.Db
{
    public sealed class Database
    {
        private static IDatabase Instance { get; set; }

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

        //public static T Select<T>(string id) where T : class => Instance.Select<T>(id: id);

        public static T Select<T>(params object[] args) where T : class => Instance.Select<T>(args: args);

        public static IEnumerable<T> SelectAll<T>() where T : class => Instance.SelectAll<T>();

        public static bool Contains<T>(params object[] args) where T : class => Instance.Contains<T>(args: args);

        public static IEnumerable<T> Where<T>(Expression<Func<T, bool>> predicate) where T : class =>
            Instance.SelectAll<T>().AsQueryable().Where(predicate);

        public static void Insert<T>(T entity) where T : class => Instance.Insert(entity);

        public static void Update<T>(T entity) where T : class => Instance.Update(entity);

        public static void Delete<T>(T entity) where T : class => Instance.Delete(entity);
    }
}