using System.Threading.Tasks;
using Tenjin.Contracts;
using Tenjin.Helpers;
using Tenjin.Sys.Contracts.Interfaces;

namespace Tenjin.Sys.Contracts
{
    public class SysDbBuilder : BaseDbBuilder, ISysDbBuilder
    {
        public override Task<string> GetConnectionString()
        {
            var connection = AppSettings.Instance.Get("SYS_CONNECTION_STRING") ?? "mongodb://localhost:27017";
            return Task.FromResult(connection);
        }

        public override Task<string> GetDatabaseName()
        {
            var database = AppSettings.Instance.Get("SYS_DB_NAME") ?? "tenjin_course";
            return Task.FromResult(database);
        }
    }
}
