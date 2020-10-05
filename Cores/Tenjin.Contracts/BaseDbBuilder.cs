using System.Threading.Tasks;
using Tenjin.Contracts.Interfaces;
using Tenjin.Helpers;

namespace Tenjin.Contracts
{
    public class BaseDbBuilder : IDbBuilder
    {
        public virtual Task<string> GetConnectionString()
        {
            var connection = AppSettings.Instance.Get("CONNECTION_STRING") ?? "mongodb://localhost:27017";
            return Task.FromResult(connection);
        }

        public virtual Task<string> GetDatabaseName()
        {
            var database = AppSettings.Instance.Get("DATABASE") ?? "master";
            return Task.FromResult(database);
        }
    }
}
