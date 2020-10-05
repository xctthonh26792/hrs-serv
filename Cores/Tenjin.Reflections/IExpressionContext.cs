using MongoDB.Driver;
using Tenjin.Models.Interfaces;

namespace Tenjin.Reflections
{
    public interface IExpressionContext<T>
        where T : IEntity
    {
        FilterDefinition<T> GetPreExpression();
        SortDefinition<T> GetSort();
    }

    public interface IExpressionContext<T, TV>
        where T : IEntity
        where TV : T
    {
        FilterDefinition<T> GetPreExpression();
        FilterDefinition<TV> GetPostExpression();
        SortDefinition<T> GetSort();
    }
}
