using MongoDB.Driver;
using Tenjin.Models.Entities;

namespace Tenjin.Reflections
{
    public class ExpressionContext<T> : IExpressionContext<T>
        where T : BaseEntity
    {
        private readonly FilterDefinition<T> _filter;
        private readonly SortDefinition<T> _sort;
        public ExpressionContext(FilterDefinition<T> filter, SortDefinition<T> sort = null)
        {
            _filter = filter;
            _sort = sort;
        }

        public FilterDefinition<T> GetPreExpression()
        {
            return _filter;
        }

        public SortDefinition<T> GetSort()
        {
            return _sort;
        }
    }

    public class ExpressionContext<T, TV> : IExpressionContext<T, TV>
        where T : BaseEntity
        where TV : T
    {
        private readonly FilterDefinition<T> _filter;
        private readonly SortDefinition<T> _sort;
        public ExpressionContext(FilterDefinition<T> filter, SortDefinition<T> sort = null)
        {
            _filter = filter;
            _sort = sort;
        }

        public FilterDefinition<TV> GetPostExpression()
        {
            return Builders<TV>.Filter.Where(x => true);
        }

        public FilterDefinition<T> GetPreExpression()
        {
            return _filter;
        }

        public SortDefinition<T> GetSort()
        {
            return _sort;
        }
    }
}
