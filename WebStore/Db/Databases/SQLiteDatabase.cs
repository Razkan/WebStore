using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebStore.Db.Attribute;

namespace WebStore.Db
{
    // TODO Update to prepared statements https://github.com/OWASP/CheatSheetSeries/blob/d8edfc0659e986829dec36ee0ee093688f0bf694/cheatsheets/Query_Parameterization_Cheat_Sheet.md
    public class SQLiteDatabase : IDatabase
    {
        private const string INTEGER = nameof(INTEGER);
        private const string TEXT = nameof(TEXT);
        private const string Id = nameof(Id);

        private IDatabaseConfiguration Configuration { get; }

        private static readonly Type[] StringType = {typeof(string)};

        public SQLiteDatabase(IDatabaseConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void Run()
        {
            Setup();
            CreateTables();
        }

        private void Setup()
        {
            if (!Directory.Exists(Configuration.Directory)) Directory.CreateDirectory(Configuration.Directory);
            if (!File.Exists(Configuration.File)) SQLiteConnection.CreateFile(Configuration.File);
        }

        private void CreateTables()
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies()
                .SingleOrDefault(a => a.GetName().Name == nameof(WebStore));
            if (assembly == null) return;

            foreach (var type in GetTypesWithHelpAttribute(assembly))
            {
                try
                {
                    var sql = $"CREATE TABLE IF NOT EXISTS {type.Name} (" +
                              $"{CreateQueryFromProperties(type)});";
                    var task = ExecuteNonQueryAsync(sql);
                    Task.WaitAll(task);
                }
#pragma warning disable 168
                catch (Exception e)
#pragma warning restore 168
                {
                    // TODO log errors
                }
            }
        }

        private static IEnumerable<Type> GetTypesWithHelpAttribute(Assembly assembly) => assembly.GetTypes()
            .Where(type => type.GetCustomAttributes(typeof(TableAttribute), true).Length > 0);

        private static string CreateQueryFromProperties(Type t) => string.Join(", ",
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

        public async Task<T> SelectAsync<T>(params object[] args) where T : class
        {
            var sql = $"SELECT * FROM {typeof(T).Name} " +
                      $"WHERE {string.Join(" AND ", args)};";
            return await ExecuteReaderAsync(sql, async reader =>
            {
                await reader.ReadAsync();
                return reader.HasRows ? await ToObjectAsync<T>(reader) : default;
            }).Unwrap();
        }

        public async Task<IEnumerable<T>> SelectAllAsync<T>() where T : class
        {
            var sql = $"SELECT * FROM {typeof(T).Name};";
            return await ExecuteReaderAsync(sql, async reader =>
            {
                var list = new List<T>();
                while (await reader.ReadAsync())
                {
                    list.Add(await ToObjectAsync<T>(reader));
                }

                return list;
            }).Unwrap();
        }

        public async Task<IEnumerable<T>> SelectAllAsync<T>(params object[] args) where T : class
        {
            var sql = $"SELECT * FROM {typeof(T).Name} " +
                      $"WHERE {string.Join(" AND ", args)};";
            return await ExecuteReaderAsync(sql, async reader =>
            {
                var list = new List<T>();
                while (await reader.ReadAsync())
                {
                    list.Add(await ToObjectAsync<T>(reader));
                }

                return list;
            }).Unwrap();
        }

        // ReSharper disable once PossiblyMistakenUseOfParamsMethod
        public async Task<bool> ContainsAsync<T>(string id) where T : class =>
            await ContainsAsync<T>(args: $"id='{id}'");

        public Task<bool> ContainsAsync<T>(params object[] args) where T : class
        {
            var sql = $"SELECT * FROM {typeof(T).Name} " +
                      $"WHERE {string.Join(" AND ", args)} " +
                      "LIMIT 1;";
            return ExecuteReaderAsync(sql, async reader =>
            {
                await reader.ReadAsync();
                return reader.HasRows;
            }).Unwrap();
        }

        public async Task InsertAsync<T>(T entity) where T : class
        {
            await RecursiveInsert(entity);
            var type = typeof(T);
            var sql =
                $"INSERT OR IGNORE INTO {type.Name}({GetPropertyNames(type)}) " +
                $"VALUES('{GetPropertyValues(entity)}');";
            await ExecuteNonQueryAsync(sql);
        }


        /// <summary>
        /// Recursively adds internal objects not yet stored in the db.
        /// Example: new Account(..., ..., new User()), first User would be stored in the db, then Account. 
        /// </summary>
        private async Task RecursiveInsert(object entity)
        {
            foreach (var pi in entity.GetType().GetProperties())
            {
                if (IsForeignKey(pi))
                {
                    var obj = pi.GetValue(entity);
                    if (!await InternalContainsAsync(obj)) await InternalInsertAsync(obj);
                }
            }
        }

        public async Task UpdateAsync<T>(T entity) where T : class
        {
            var type = typeof(T);
            var sql =
                $"UPDATE {type.Name} " + // UPDATE User
                $"SET {GetPropertyValuePair(entity)} " + // SET Id='5', FirstName='John', LastName='Doe'
                $"WHERE Id='{GetPropertyId(entity)}';"; // WHERE Id=5
            await ExecuteNonQueryAsync(sql);
        }

        public async Task DeleteAsync<T>(T entity) where T : class
        {
            var type = typeof(T);
            var sql =
                $"DELETE FROM {type.Name} " + // DELETE FROM User
                $"WHERE Id='{GetPropertyId(entity)}';"; // WHERE Id=5 

            await ExecuteNonQueryAsync(sql);
        }

        private async Task<T> ExecuteReaderAsync<T>(string sql, Func<DbDataReader, T> func)
        {
            using (var conn = new SQLiteConnection(Configuration.Args))
            {
                await conn.OpenAsync();
                var cmd = new SQLiteCommand(sql, conn);
                try
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        return func(reader);
                    }
                }
#pragma warning disable 168
                catch (Exception e)
#pragma warning restore 168
                {
                    // TODO log errors
                    // Log errors
                    return default;
                }
            }
        }

        private async Task ExecuteNonQueryAsync(string sql)
        {
            using (var conn = new SQLiteConnection(Configuration.Args))
            {
                await conn.OpenAsync();
                var cmd = new SQLiteCommand(sql, conn);
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
#pragma warning disable 168
                catch (Exception e)
#pragma warning restore 168
                {
                    // TODO log errors
                    // Log errors
                }
            }
        }

        private async Task<T> ToObjectAsync<T>(IDataRecord reader) where T : class
        {
            var t = Activator.CreateInstance<T>();
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var pi = t.GetType().GetProperty(reader.GetName(i));
                if (pi == null) continue;
                pi.SetValue(t, await ToObject(reader.GetValue(i)));

                async Task<object> ToObject(object value)
                {
                    switch (pi)
                    {
                        case var _ when IsDateTime(pi):
                            return DateTime.Parse(value.ToString());
                        case var _ when IsTimeSpan(pi):
                            return TimeSpan.Parse(value.ToString());
                        case var _ when IsForeignKey(pi):
                            return await InternalSelectAsync(pi, value);
                        case var _ when IsUShort(pi):
                            return ushort.Parse(value.ToString());
                        case var _ when IsBool(pi):
                            return bool.Parse(value.ToString());
                        default:
                            return value;
                    }
                }
            }

            return t;
        }

        private Task<bool> InternalContainsAsync(object obj) => Task.Run(() => Convert.ToBoolean(GetType()
            .GetMethod(nameof(ContainsAsync), StringType)
            ?.MakeGenericMethod(obj.GetType())
            .Invoke(this, new object[] {GetPropertyId(obj)})));

        private Task InternalInsertAsync(object obj) => Task.Run(() => GetType()
            .GetMethod(nameof(InsertAsync))?
            .MakeGenericMethod(obj.GetType())
            .Invoke(this, new[] {obj}));

        private Task<object> InternalSelectAsync(PropertyInfo pi, object value) => Task.Run(() => GetType()
            .GetMethod(nameof(SelectAsync), StringType)
            ?.MakeGenericMethod(pi.PropertyType)
            .Invoke(this, new[] {value}));

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

        private static bool IsUShort(PropertyInfo pi) => pi.PropertyType == typeof(ushort);

        private static bool IsBool(PropertyInfo pi) => pi.PropertyType == typeof(bool);

        private static bool IsTimeSpan(PropertyInfo pi) => pi.PropertyType == typeof(TimeSpan);

        private static bool IsPrimaryKey(PropertyInfo pi) =>
            pi.GetCustomAttributes(typeof(PrimaryKeyAttribute), true).Length > 0;

        private static bool IsForeignKey(PropertyInfo pi) =>
            pi.GetCustomAttributes(typeof(ForeignKeyAttribute), true).Length > 0;

        private static bool IsUnique(PropertyInfo pi) =>
            pi.GetCustomAttributes(typeof(UniqueAttribute), true).Length > 0;

        public static IDatabase Make(IDatabaseConfiguration configuration) => new SQLiteDatabase(configuration);
    }
}