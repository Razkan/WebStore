namespace WebStore.Db
{
    public interface IDatabaseConfiguration
    {
        string Directory { get; }
        string Name { get; }
        string File { get; }
        string Args { get; }
    }

    public class SqLiteConfiguration : IDatabaseConfiguration
    {
        public string Directory { get; set; }
        public string Name { get; set; }
        public string File { get; set; }
        public string Args { get; set; }

        public static IDatabaseConfiguration Make(string directory, string name, string file, string args)
        {
            return new SqLiteConfiguration
            {
                Directory = directory,
                Name = name,
                File = file,
                Args = args
            };
        }
    }

    public class MySqlConfiguration : IDatabaseConfiguration
    {
        public string Directory { get; set; }
        public string Name { get; set; }
        public string File { get; set; }
        public string Args { get; set; }


        public static IDatabaseConfiguration Make(string directory, string name, string file, string args)
        {
            return new MySqlConfiguration
            {
                Directory = directory,
                Name = name,
                Args = args
            };
        }
    }
}