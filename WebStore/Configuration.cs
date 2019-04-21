using System;
using System.IO;
using Newtonsoft.Json;

namespace WebStore
{
    public sealed class Configuration
    {
        internal const string ConfigPath = "WEBSTORE_CONFIG";

        internal static dynamic Config { get; set; }

        public static void Register()
        {
            Update();
        }

        public static void Update()
        {
            Config = JsonConvert.DeserializeObject(File.ReadAllText(
                Environment.GetEnvironmentVariable(ConfigPath, EnvironmentVariableTarget.Machine) ??
                throw new Exception($"Missing '{ConfigPath}' environment variable")));
        }
    }
}