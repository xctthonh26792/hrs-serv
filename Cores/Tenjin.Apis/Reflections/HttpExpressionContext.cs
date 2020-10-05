using MongoDB.Driver;
using Tenjin.Apis.Reflections.Interfaces;
using Tenjin.Models.Entities;
using Tenjin.Reflections;

namespace Tenjin.Apis.Reflections
{
    public class HttpExpressionContext<T> : IExpressionContext<T>
        where T : BaseEntity
    {
        private readonly IHttpExpressionReader _reader;
        public HttpExpressionContext(IHttpExpressionReader reader)
        {
            _reader = reader;
        }

        public FilterDefinition<T> GetPreExpression()
        {
            return _reader.GetPreExpression<T>();
        }

        public SortDefinition<T> GetSort()
        {
            return _reader.GetSort<T>();
        }
    }

    public class HttpExpressionContext<T, TV> : IExpressionContext<T, TV>
        where T : BaseEntity
        where TV : T
    {
        private readonly IHttpExpressionReader _reader;
        public HttpExpressionContext(IHttpExpressionReader reader)
        {
            _reader = reader;
        }

        public FilterDefinition<TV> GetPostExpression()
        {
            return _reader.GetPostExpression<TV>();
        }

        public FilterDefinition<T> GetPreExpression()
        {
            return _reader.GetPreExpression<T>();
        }

        public SortDefinition<T> GetSort()
        {
            return _reader.GetSort<T>();
        }
    }
}
