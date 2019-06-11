using System.IO;
using Newtonsoft.Json;

namespace WebStore
{
    public sealed class Configuration
    {
        private static readonly string ConfigFilePath = GetExecutableFolder() + "\\config.json";

        private static dynamic Config { get; set; }

        internal static dynamic Databases => Config.Databases;

        public static void Register() => Update();

        public static void Update() => Config = JsonConvert.DeserializeObject(File.ReadAllText(ConfigFilePath));

        private static string GetExecutableFolder() => Path.GetDirectoryName(
            System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)?.Substring(6);
    }
}