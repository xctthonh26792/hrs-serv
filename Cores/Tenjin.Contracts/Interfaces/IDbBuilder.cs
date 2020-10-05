using System.Threading.Tasks;

namespace Tenjin.Contracts.Interfaces
{
    public interface IDbBuilder
    {
        Task<string> GetConnectionString();

        Task<string> GetDatabaseName();
    }
}
