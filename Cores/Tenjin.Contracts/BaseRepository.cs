using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Tenjin.Contracts.Interfaces;
using Tenjin.Helpers;
using Tenjin.Models.Entities;

namespace Tenjin.Contracts
{
    public class BaseRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<T> _collection;
        public BaseRepository(IMongoDatabase database)
        {
            _database = database;
            _collection = _database.GetCollection<T>();
        }

        public async Task Add(T entity)
        {
            if (entity == null)
            {
                return;
            }
            await _collection.InsertOneAsync(entity);
        }

        public async Task AddMany(IEnumerable<T> entities)
        {
            if (entities == null)
            {
                return;
            }
            await _collection.InsertManyAsync(entities);
        }

        public async Task BulkWrite(IEnumerable<WriteModel<T>> requests)
        {
            await _collection.BulkWriteAsync(requests);
        }

        public async Task<long> Count(Expression<Func<T, bool>> filter)
        {
            return await _collection.CountDocumentsAsync(CombineExpression(filter));
        }

        public async Task<long> Count(FilterDefinition<T> filter)
        {
            return await _collection.CountDocumentsAsync(CombineExpression(filter));
        }

        public async Task Delete(T entity)
        {
            if (entity == null)
            {
                return;
            }
            await _collection.DeleteOneAsync(CombineExpression(Builders<T>.Filter.Eq(x => x.Id, entity.Id)));
        }

        public async Task Delete(Expression<Func<T, bool>> filter)
        {
            await _collection.DeleteOneAsync(CombineExpression(filter));
        }

        public async Task Delete(FilterDefinition<T> filter)
        {
            await _collection.DeleteOneAsync(CombineExpression(filter));
        }

        public async Task DeleteMany(Expression<Func<T, bool>> filter)
        {
            await _collection.DeleteManyAsync(CombineExpression(filter));
        }

        public async Task DeleteMany(FilterDefinition<T> filter)
        {
            await _collection.DeleteManyAsync(CombineExpression(filter));
        }

        public virtual IAggregateFluent<T> GetAggregate()
        {
            return _collection.Aggregate();
        }

        public async Task<IEnumerable<T>> GetByExpression(Expression<Func<T, bool>> filter, SortDefinition<T> sort = null, ProjectionDefinition<T> projection = null)
        {
            var fluent = CreateFluent(filter, sort, projection);
            return await fluent.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetByExpression(FilterDefinition<T> filter, SortDefinition<T> sort = null, ProjectionDefinition<T> projection = null)
        {
            var fluent = CreateFluent(filter, sort, projection);
            return await fluent.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetPageByExpression(int page, int quantity, Expression<Func<T, bool>> filter, SortDefinition<T> sort = null, ProjectionDefinition<T> projection = null)
        {
            var fluent = CreateFluent(filter, sort, projection);
            return await fluent.Skip(Math.Max(0, page - 1) * quantity).Limit(quantity).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetPageByExpression(int page, int quantity, FilterDefinition<T> filter, SortDefinition<T> sort = null, ProjectionDefinition<T> projection = null)
        {
            var fluent = CreateFluent(filter, sort, projection);
            return await fluent.Skip(Math.Max(0, page - 1) * quantity).Limit(quantity).ToListAsync();
        }

        public IMongoCollection<T> GetCollection()
        {
            return _collection;
        }

        public async Task<T> GetSingleByExpression(Expression<Func<T, bool>> filter, SortDefinition<T> sort = null, ProjectionDefinition<T> projection = null, int index = 1)
        {
            var fluent = CreateFluent(filter, sort, projection);
            return await fluent.Skip(index - 1).Limit(1).FirstOrDefaultAsync();
        }

        public async Task<T> GetSingleByExpression(FilterDefinition<T> filter, SortDefinition<T> sort = null, ProjectionDefinition<T> projection = null, int index = 1)
        {
            var fluent = CreateFluent(filter, sort, projection);
            return await fluent.Skip(index - 1).Limit(1).FirstOrDefaultAsync();
        }

        public async Task Replace(T entity)
        {
            if (entity == null)
            {
                return;
            }
            var filter = Builders<T>.Filter.Eq(x => x.Id, entity.Id);
            await _collection.ReplaceOneAsync(CombineExpression(filter), entity);
        }

        public async Task<T> UpsertOne(Expression<Func<T, bool>> filter, UpdateDefinition<T> updater)
        {
            var options = new FindOneAndUpdateOptions<T, T> { IsUpsert = true, ReturnDocument = ReturnDocument.After };
            return await _collection.FindOneAndUpdateAsync(CombineExpression(filter), updater, options);
        }

        public async Task<T> UpsertOne(FilterDefinition<T> filter, UpdateDefinition<T> updater)
        {
            var options = new FindOneAndUpdateOptions<T, T> { IsUpsert = true, ReturnDocument = ReturnDocument.After };
            return await _collection.FindOneAndUpdateAsync(CombineExpression(filter), updater, options);
        }

        public async Task UpdateOne(Expression<Func<T, bool>> filter, UpdateDefinition<T> updater, bool upsert = false)
        {
            var options = new UpdateOptions { IsUpsert = upsert };
            await _collection.UpdateOneAsync(CombineExpression(filter), updater, options);
        }

        public async Task UpdateOne(FilterDefinition<T> filter, UpdateDefinition<T> updater, bool upsert = false)
        {
            var options = new UpdateOptions { IsUpsert = upsert };
            await _collection.UpdateOneAsync(CombineExpression(filter), updater, options);
        }

        public async Task UpdateMany(Expression<Func<T, bool>> filter, UpdateDefinition<T> updater, bool upsert = false)
        {
            var options = new UpdateOptions { IsUpsert = upsert };
            await _collection.UpdateManyAsync(CombineExpression(filter), updater, options);
        }

        public async Task UpdateMany(FilterDefinition<T> filter, UpdateDefinition<T> updater, bool upsert = false)
        {
            var options = new UpdateOptions { IsUpsert = upsert };
            await _collection.UpdateManyAsync(CombineExpression(filter), updater, options);
        }

        #region Functions
        private IFindFluent<T, T> CreateFluent(Expression<Func<T, bool>> filter, SortDefinition<T> sort, ProjectionDefinition<T> projection)
        {
            var fluent = _collection.Find(CombineExpression(filter));
            if (projection != null)
            {
                fluent.Options.Projection = Builders<T>.Projection.Combine(projection);
            }
            if (sort != null)
            {
                fluent.Sort(sort);
            }
            return fluent;
        }

        private IFindFluent<T, T> CreateFluent(FilterDefinition<T> filter, SortDefinition<T> sort, ProjectionDefinition<T> projection)
        {
            var fluent = _collection.Find(CombineExpression(filter));
            if (projection != null)
            {
                fluent.Options.Projection = Builders<T>.Projection.Combine(projection);
            }
            if (sort != null)
            {
                fluent.Sort(sort);
            }
            return fluent;
        }

        #endregion

        #region Extends Functions
        protected virtual FilterDefinition<T> CombineExpression(FilterDefinition<T> filter)
        {
            return filter;
        }
        #endregion
    }
}
