using MongoDB.Driver;
using Tenjin.Models.Entities;

namespace Tenjin.Apis.Reflections.Interfaces
{
    public interface IHttpExpressionReader
    {
        FilterDefinition<T> GetPreExpression<T>()
            where T : BaseEntity;

        FilterDefinition<T> GetPostExpression<T>()
            where T : BaseEntity;

        SortDefinition<T> GetSort<T>()
            where T : BaseEntity;
    }
}
