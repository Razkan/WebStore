//using Microsoft.AspNetCore;
//using Microsoft.AspNetCore.Hosting;
//using WebStore.Db;

//namespace WebStore
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            CreateDatabase().Run();
//            CreateWebHostBuilder(args).Build().Run();
//        }

//        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
//            WebHost.CreateDefaultBuilder(args)
//                .UseStartup<Startup>();

//        public static IDatabase CreateDatabase() => Database.CreateDefaultDatabase();
//    }
//}