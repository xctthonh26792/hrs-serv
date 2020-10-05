using Tenjin.Models.Entities;

namespace Tenjin.Contracts.Interfaces
{
    public interface IContext
    {
        IRepository<T> ResolveRepository<T>() where T : BaseEntity;
    }
}
