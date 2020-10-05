using MongoDB.Driver;
using System.Threading.Tasks;
using Tenjin.Contracts.Interfaces;
using Tenjin.Models.Entities;

namespace Tenjin.Contracts
{
    public class BaseContext : IContext
    {
        private readonly IMongoDatabase _database;

        protected BaseContext(IDbBuilder builder)
        {
            _database = GetDatabase(builder).GetAwaiter().GetResult();
        }

        protected IMongoDatabase GetDatabase()
        {
            return _database;
        }

        private async Task<IMongoDatabase> GetDatabase(IDbBuilder builder)
        {
            IMongoClient client = new MongoClient(await builder.GetConnectionString());
            return client.GetDatabase(await builder.GetDatabaseName());
        }

        public virtual IRepository<T> ResolveRepository<T>() where T : BaseEntity
        {
            return new BaseRepository<T>(_database);
        }
    }
}
