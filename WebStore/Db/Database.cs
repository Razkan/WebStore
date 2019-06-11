using System;
using System.Collections.Generic;

namespace WebStore.Db
{
    public static class Database
    {
        private const string SQLite = nameof(SQLite);
        private const string MySQL = nameof(MySQL);

        private static ICollection<IDatabase> Instances { get; set; }

        internal static IDatabase Instance { get; private set; }

        public static void Register()
        {
            Setup();
            Run();

            //var good = new Good { Id = "abc" };
            //Instance.Insert(good);

            //var good = Select<Good>("abc");
            //var stopwatch = new Stopwatch();
            //Instance.Insert(
            //new Account("kalle", "123", new User {Id = "dd", FirstName = "Frank", LastName = "Larsson"}));
            //var user = Instance.Select<User>("dd");
        }

        private static void Setup()
        {
            Instances = new List<IDatabase>();
            foreach (var database in Configuration.Databases)
            {
                string type = database.Type;
                string directory = database.Directory;
                string name = database.Name;
                string args = database.Args;
                var file = directory + "\\" + name;

                switch (type)
                {
                    case SQLite:
                        Instances.Add(SQLiteDatabase.Make(SqLiteConfiguration.Make(directory, name, file, args)));
                        break;
                    case MySQL:
                        Instances.Add(MySQLDatabase.Make(MySqlConfiguration.Make(directory, name, file, args)));
                        break;
                    default: throw new ArgumentOutOfRangeException(type);
                }
            }

            Instance = new MultiDatabaseHandler(Instances);
        }

        private static void Run() => Instance.Run();
    }
}