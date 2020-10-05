using System.Threading.Tasks;
using Tenjin.Sys.Models.Cores;

namespace Tenjin.Sys.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<Dashboard> GetDashboard();
    }
}
