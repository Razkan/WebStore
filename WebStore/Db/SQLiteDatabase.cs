using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using static WebStore.Configuration;

namespace WebStore.Db
{
    public class SQLiteDatabase : IDatabase
    {
        private const string INTEGER = nameof(INTEGER);
        private const string TEXT = nameof(TEXT);
        private const string Id = nameof(Id);

        private static string DbDirectory;
        private static string DbName;
        private static string DbFile;

        private static string DbArgs;

        private static readonly Type[] StringType = {typeof(string)};

        public void Run()
        {
            Setup();
            CreateTable();
        }

        private void Setup()
        {
            DbDirectory = Config.Database.Directory;
            DbName = Config.Database.Name;
            DbFile = DbDirectory + "\\" + DbName;
            DbArgs = Config.Database.Args;

            if (!Directory.Exists(DbDirectory)) Directory.CreateDirectory(DbDirectory);
            if (!File.Exists(DbFile)) SQLiteConnection.CreateFile(DbFile);
        }

        private static void CreateTable()
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies()
                .SingleOrDefault(a => a.GetName().Name == nameof(WebStore));
            if (assembly == null) return;

            foreach (var type in GetTypesWithHelpAttribute(assembly))
            {
                try
                {
                    var sql = $"CREATE TABLE IF NOT EXISTS {type.Name} (" +
                              $"{PropertiesToColumns(type)});";
                    ExecuteNonQuery(sql);
                }
                catch (Exception e)
                {
                }
            }
        }

        private static IEnumerable<Type> GetTypesWithHelpAttribute(Assembly assembly) => assembly.GetTypes()
            .Where(type => type.GetCustomAttributes(typeof(TableAttribute), true).Length > 0);

        private static string PropertiesToColumns(Type t) => string.Join(", ",
            t.GetProperties().Select(e =>
            {
                if (IsPrimaryKey(e)) return $"[{e.Name}] TEXT NON NULL PRIMARY KEY";
                if (IsForeignKey(e)) return $"[{e.Name}] TEXT NON NULL";
                if (IsUnique(e)) return $"[{e.Name}] TEXT NON NULL UNIQUE";
                return $"[{e.Name}] {ToSQLType(e.PropertyType)} NON NULL";
            }));


        private static string ToSQLType(Type t)
        {
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return INTEGER;
                default:
                    return TEXT;
            }
        }

        // ReSharper disable once PossiblyMistakenUseOfParamsMethod
        public T Select<T>(string id) where T : class => Select<T>(args: $"id='{id}'");

        public T Select<T>(params object[] args) where T : class
        {
            var sql = $"SELECT * FROM {typeof(T).Name} " +
                      $"WHERE {string.Join(" AND ", args)};";
            return ExecuteReader(sql, reader =>
            {
                reader.Read();
                return reader.HasRows ? ToObject<T>(reader) : default;
            });
        }

        public IEnumerable<T> SelectAll<T>() where T : class
        {
            var sql = $"SELECT * FROM {typeof(T).Name};";
            return ExecuteReader(sql, reader =>
            {
                var list = new List<T>();
                while (reader.Read())
                {
                    list.Add(ToObject<T>(reader));
                }

                return list;
            });
        }

        // ReSharper disable once PossiblyMistakenUseOfParamsMethod
        public bool Contains<T>(string id) where T : class => Contains<T>(args: $"id='{id}'");

        public bool Contains<T>(params object[] args) where T : class
        {
            var sql = $"SELECT * FROM {typeof(T).Name} " +
                      $"WHERE {string.Join(" AND ", args)} " +
                      "LIMIT 1;";
            return ExecuteReader(sql, reader =>
            {
                reader.Read();
                return reader.HasRows;
            });
        }

        public void Insert<T>(T entity) where T : class
        {
            RecursiveInsert(entity);
            var type = typeof(T);
            var sql =
                $"INSERT OR IGNORE INTO {type.Name}({GetPropertyNames(type)}) " +
                $"VALUES('{GetPropertyValues(entity)}');";
            ExecuteNonQuery(sql);
        }


        /// <summary>
        /// Recursively adds internal objects not yet stored in the db.
        /// Example: new Account(..., ..., new User()), first User would be stored in the db, then Account. 
        /// </summary>
        private void RecursiveInsert(object entity)
        {
            foreach (var pi in entity.GetType().GetProperties())
            {
                if (IsForeignKey(pi))
                {
                    var obj = pi.GetValue(entity);
                    if (!InternalContains(obj)) InternalInsert(obj);
                }
            }
        }

        public void Update<T>(T entity) where T : class
        {
            var type = typeof(T);
            var sql =
                $"UPDATE {type.Name} " + // UPDATE User
                $"SET {GetPropertyValuePair(entity)} " + // SET Id='5', FirstName='John', LastName='Doe'
                $"WHERE Id='{GetPropertyId(entity)}';"; // WHERE Id=5
            ExecuteNonQuery(sql);
        }

        public void Delete<T>(T entity) where T : class
        {
            //DELETE FROM Students WHERE StudentId = 11 OR StudentId = 12;
            var type = typeof(T);
            var sql =
                $"DELETE FROM {type.Name} " + // DELETE FROM User
                $"WHERE Id='{GetPropertyId(entity)}';"; // WHERE Id=5 

            ExecuteNonQuery(sql);
        }

        private static T ExecuteReader<T>(string sql, Func<SQLiteDataReader, T> func)
        {
            using (var conn = new SQLiteConnection(DbArgs))
            {
                conn.Open();
                var cmd = new SQLiteCommand(sql, conn);
                try
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        return func(reader);
                    }
                }
                catch (Exception e)
                {
                    // Log errors
                    return default;
                }
            }
        }

        private static void ExecuteNonQuery(string sql)
        {
            using (var conn = new SQLiteConnection(DbArgs))
            {
                conn.Open();
                var cmd = new SQLiteCommand(sql, conn);
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    // Log errors
                }
            }
        }

        private T ToObject<T>(SQLiteDataReader reader) where T : class
        {
            var t = Activator.CreateInstance<T>();
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var pi = t.GetType().GetProperty(reader.GetOriginalName(i));
                if (pi == null) continue;
                pi.SetValue(t, ToObject(reader.GetValue(i)));

                object ToObject(object value)
                {
                    switch (pi)
                    {
                        case var _ when IsDateTime(pi):
                            return DateTime.Parse(value.ToString());
                        case var _ when IsTimeSpan(pi):
                            return TimeSpan.Parse(value.ToString());
                        case var _ when IsForeignKey(pi):
                            return InternalSelect(pi, value);
                        default:
                            return value;
                    }
                }
            }

            return t;
        }

        private bool InternalContains(object obj) => Convert.ToBoolean(GetType()
            .GetMethod(nameof(Contains), StringType)
            ?.MakeGenericMethod(obj.GetType())
            .Invoke(this, new object[] {GetPropertyId(obj)}));

        private void InternalInsert(object obj) => GetType()
            .GetMethod(nameof(Insert))?
            .MakeGenericMethod(obj.GetType())
            .Invoke(this, new[] {obj});

        private object InternalSelect(PropertyInfo pi, object value) => GetType()
            .GetMethod(nameof(Select), StringType)
            ?.MakeGenericMethod(pi.PropertyType)
            .Invoke(this, new[] {value});

        private static string GetPropertyNames(Type type) =>
            string.Join(", ", type.GetProperties().Select(e => e.Name));

        private static string GetPropertyValues<T>(T entity) where T : class =>
            string.Join("', '",
                entity.GetType().GetProperties().Select(e =>
                    !IsForeignKey(e) ? e.GetValue(entity) : GetPropertyId(e.GetValue(entity))));

        private static string GetPropertyValuePair<T>(T entity) where T : class => string.Join(", ",
            entity.GetType().GetProperties().Select(e =>
                !IsForeignKey(e)
                    ? $"{e.Name}='{e.GetValue(entity)}'"
                    : $"{e.Name}='{GetPropertyId(e.GetValue(entity))}'"));

        private static string GetPropertyId(object entity) =>
            entity.GetType().GetProperty(Id)?.GetValue(entity).ToString();

        private static bool IsDateTime(PropertyInfo pi) => pi.PropertyType == typeof(DateTime);

        private static bool IsTimeSpan(PropertyInfo pi) => pi.PropertyType == typeof(TimeSpan);

        private static bool IsPrimaryKey(PropertyInfo pi) =>
            pi.GetCustomAttributes(typeof(PrimaryKeyAttribute), true).Length > 0;

        private static bool IsForeignKey(PropertyInfo pi) =>
            pi.GetCustomAttributes(typeof(ForeignKeyAttribute), true).Length > 0;

        private static bool IsUnique(PropertyInfo pi) =>
            pi.GetCustomAttributes(typeof(UniqueAttribute), true).Length > 0;
    }
}