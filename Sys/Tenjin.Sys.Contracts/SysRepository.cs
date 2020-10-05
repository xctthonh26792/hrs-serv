using MongoDB.Driver;
using Tenjin.Contracts;
using Tenjin.Models.Entities;

namespace Tenjin.Sys.Contracts
{
    public class SysRepository<T> : BaseRepository<T>
        where T : BaseEntity
    {
        public SysRepository(IMongoDatabase database) : base(database)
        {
        }
    }
}
