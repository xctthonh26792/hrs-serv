using System.Collections.Generic;
using System.Threading.Tasks;
using Tenjin.Services.Interfaces;
using Tenjin.Sys.Models.Cores;
using Tenjin.Sys.Models.Entities;
using Tenjin.Sys.Models.Views;

namespace Tenjin.Sys.Services.Interfaces
{
    public interface IIntershipService : IBaseService<Intership, IntershipView>
    {
        Task<bool> Validate(Intership entity);
        Task<IntershipData> GetDataForIntershipAction();

        Task Import(IEnumerable<Intership> entities);
    }
}
