using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Tenjin.Helpers;

namespace Tenjin.Sys.Apis
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(options =>
                    options.SetBasePath(TenjinUtils.EntryLocation)
                       .AddJsonFile("hosting.json", optional: true)
                       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                       .AddEnvironmentVariables()
                )
                .ConfigureWebHostDefaults(options =>
                    options
                        .UseIISIntegration()
                        .UseStartup<Startup>()
                );
    }
}
