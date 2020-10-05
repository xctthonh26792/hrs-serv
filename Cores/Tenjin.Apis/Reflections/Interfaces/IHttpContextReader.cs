using Tenjin.Models.Entities;
using Tenjin.Reflections;

namespace Tenjin.Apis.Reflections.Interfaces
{
    public interface IHttpContextReader
    {
        IExpressionContext<T> As<T>()
            where T : BaseEntity;

        IExpressionContext<T, TV> As<T, TV>()
            where T : BaseEntity
            where TV : T;
    }
}
