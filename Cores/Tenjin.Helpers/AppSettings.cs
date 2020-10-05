using Microsoft.Extensions.Configuration;

namespace Tenjin.Helpers
{
    public class AppSettings
    {
        private static AppSettings _settings;

        public AppSettings()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(TenjinUtils.EntryLocation)
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        public static AppSettings Instance => _settings ?? (_settings = new AppSettings());

        public string Get(string key)
        {
            return Configuration[$"AppSettings:{key}"];
        }
    }
}
