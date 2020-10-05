using MongoDB.Driver;
using System.Threading.Tasks;
using Tenjin.Services;
using Tenjin.Sys.Contracts.Interfaces;
using Tenjin.Sys.Models.Entities;
using Tenjin.Sys.Services.Interfaces;

namespace Tenjin.Sys.Services
{
    public class LogService : BaseService<Log>, ILogService
    {
        public LogService(ISysContext context) : base(context.LogRepository)
        {
        }

        public override async Task CreateIndexes()
        {
            static CreateIndexModel<Log> CreateIndexModel(string name, IndexKeysDefinition<Log> definition)
            {
                var options = new CreateIndexOptions { Name = name };
                return new CreateIndexModel<Log>(definition, options);
            }
            var manager = GetRepository().GetCollection().Indexes;
            await manager.CreateOneAsync(CreateIndexModel("created", Builders<Log>.IndexKeys.Descending(x => x.CreatedDate)));
            await manager.CreateOneAsync(CreateIndexModel("path", Builders<Log>.IndexKeys.Ascending(x => x.Path)));
        }
    }
}
