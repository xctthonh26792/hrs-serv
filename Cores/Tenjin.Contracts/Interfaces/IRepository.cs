using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Tenjin.Models.Interfaces;

namespace Tenjin.Contracts.Interfaces
{
    public interface IRepository<T> where T : IEntity
    {
        IMongoCollection<T> GetCollection();

        IAggregateFluent<T> GetAggregate();

        Task<IEnumerable<T>> GetPageByExpression(int page, int quantity,
            Expression<Func<T, bool>> filter,
            SortDefinition<T> sort = null,
            ProjectionDefinition<T> projection = null);

        Task<IEnumerable<T>> GetPageByExpression(int page, int quantity,
            FilterDefinition<T> filter,
            SortDefinition<T> sort = null,
            ProjectionDefinition<T> projection = null);

        Task<T> GetSingleByExpression(Expression<Func<T, bool>> filter,
            SortDefinition<T> sort = null,
            ProjectionDefinition<T> projection = null, int index = 1);

        Task<IEnumerable<T>> GetByExpression(Expression<Func<T, bool>> filter,
            SortDefinition<T> sort = null,
            ProjectionDefinition<T> projection = null);

        Task<T> GetSingleByExpression(FilterDefinition<T> filter,
            SortDefinition<T> sort = null,
            ProjectionDefinition<T> projection = null, int index = 1);

        Task<IEnumerable<T>> GetByExpression(FilterDefinition<T> filter,
            SortDefinition<T> sort = null,
            ProjectionDefinition<T> projection = null);

        Task<long> Count(Expression<Func<T, bool>> filter);

        Task<long> Count(FilterDefinition<T> filter);

        Task Add(T entity);

        Task AddMany(IEnumerable<T> entities);

        Task BulkWrite(IEnumerable<WriteModel<T>> requests);

        Task Replace(T entity);

        Task<T> UpsertOne(Expression<Func<T, bool>> filter, UpdateDefinition<T> updater);

        Task<T> UpsertOne(FilterDefinition<T> filter, UpdateDefinition<T> updater);

        Task UpdateOne(Expression<Func<T, bool>> filter, UpdateDefinition<T> updater, bool upsert = false);

        Task UpdateOne(FilterDefinition<T> filter, UpdateDefinition<T> updater, bool upsert = false);

        Task UpdateMany(Expression<Func<T, bool>> filter, UpdateDefinition<T> updater, bool upsert = false);

        Task UpdateMany(FilterDefinition<T> filter, UpdateDefinition<T> updater, bool upsert = false);

        Task Delete(T entity);

        Task Delete(Expression<Func<T, bool>> filter);

        Task Delete(FilterDefinition<T> filter);

        Task DeleteMany(Expression<Func<T, bool>> filter);

        Task DeleteMany(FilterDefinition<T> filter);
    }
}
