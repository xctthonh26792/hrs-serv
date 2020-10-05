using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Tenjin.Models.Cores;
using Tenjin.Models.Interfaces;
using Tenjin.Reflections;

namespace Tenjin.Services.Interfaces
{
    public interface IBaseService<T>
        where T : IEntity
    {
        IAggregateFluent<T> GetAggregate();

        Task<long> Count(string code);
        Task<long> Count(Expression<Func<T, bool>> filter);
        Task<long> Count(FilterDefinition<T> filter);
        Task<long> Count(IExpressionContext<T> context);
        Task<T> GetByCode(string code);
        Task<T> GetSingleByExpression(Expression<Func<T, bool>> filter, SortDefinition<T> sort = null, ProjectionDefinition<T> projection = null, int index = 1);
        Task<T> GetSingleByExpression(FilterDefinition<T> filter, SortDefinition<T> sort = null, ProjectionDefinition<T> projection = null, int index = 1);
        Task<T> GetSingleByExpression(IExpressionContext<T> context, ProjectionDefinition<T> projection = null, int index = 1);
        Task<IEnumerable<T>> GetByExpression(Expression<Func<T, bool>> filter, SortDefinition<T> sort = null, ProjectionDefinition<T> projection = null);
        Task<IEnumerable<T>> GetByExpression(FilterDefinition<T> filter, SortDefinition<T> sort = null, ProjectionDefinition<T> projection = null);
        Task<IEnumerable<T>> GetByExpression(IExpressionContext<T> context, ProjectionDefinition<T> projection = null);
        Task<FetchResult<T>> GetPageByExpression(Expression<Func<T, bool>> filter, int page, int quantity = 10, SortDefinition<T> sort = null, ProjectionDefinition<T> projection = null);
        Task<FetchResult<T>> GetPageByExpression(FilterDefinition<T> filter, int page, int quantity = 10, SortDefinition<T> sort = null, ProjectionDefinition<T> projection = null);
        Task<FetchResult<T>> GetPageByExpression(IExpressionContext<T> context, int page, int quantity = 10, ProjectionDefinition<T> projection = null);
        Task Add(T entity);
        Task AddMany(IEnumerable<T> entities);
        Task Replace(T entity);
        Task Delete(string code);
        Task Published(string code);
        Task Unpublished(string code);

        Task CreateIndexes();
    }

    public interface IBaseService<T, TV>
        where T : IEntity
        where TV : T
    {
        IAggregateFluent<T> GetAggregate();

        Task<long> Count(string code);
        Task<long> Count(Expression<Func<T, bool>> filter);
        Task<long> Count(FilterDefinition<T> filter);
        Task<long> Count(IExpressionContext<T, TV> context);
        Task<TV> GetByCode(string code);
        Task<TV> GetSingleByExpression(Expression<Func<T, bool>> filter, SortDefinition<T> sort = null, ProjectionDefinition<T> projection = null, int index = 1);
        Task<TV> GetSingleByExpression(FilterDefinition<T> filter, SortDefinition<T> sort = null, ProjectionDefinition<T> projection = null, int index = 1);
        Task<TV> GetSingleByExpression(IExpressionContext<T, TV> context, ProjectionDefinition<T> projection = null, int index = 1);
        Task<IEnumerable<TV>> GetByExpression(Expression<Func<T, bool>> filter, SortDefinition<T> sort = null, ProjectionDefinition<T> projection = null);
        Task<IEnumerable<TV>> GetByExpression(FilterDefinition<T> filter, SortDefinition<T> sort = null, ProjectionDefinition<T> projection = null);
        Task<IEnumerable<TV>> GetByExpression(IExpressionContext<T, TV> context, ProjectionDefinition<T> projection = null);
        Task<FetchResult<TV>> GetPageByExpression(Expression<Func<T, bool>> filter, int page = 1, int quantity = 10, SortDefinition<T> sort = null, ProjectionDefinition<T> projection = null);
        Task<FetchResult<TV>> GetPageByExpression(FilterDefinition<T> filter, int page = 1, int quantity = 10, SortDefinition<T> sort = null, ProjectionDefinition<T> projection = null);
        Task<FetchResult<TV>> GetPageByExpression(IExpressionContext<T, TV> context, int page, int quantity = 10, ProjectionDefinition<T> projection = null);
        Task Add(T entity);
        Task AddMany(IEnumerable<T> entities);
        Task Replace(T entity);
        Task Delete(string code);
        Task Published(string code);
        Task Unpublished(string code);

        Task CreateIndexes();
    }
}
