using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Tenjin.Contracts.Interfaces;
using Tenjin.Helpers;
using Tenjin.Models.Cores;
using Tenjin.Models.Entities;
using Tenjin.Reflections;
using Tenjin.Services.Interfaces;

namespace Tenjin.Services
{
    public class BaseService
    {
        protected bool IsObjectId(string code)
        {
            return ObjectId.TryParse(code, out _);
        }

        protected Expression<Func<T, bool>> GetFilterExpression<T>(string code)
            where T : BaseEntity
        {
            Expression<Func<T, bool>> filter = x => x.Id.Equals(code);
            return filter;
        }

        protected void PrepareInsertModel<T>(T entity)
            where T : BaseEntity
        {
            entity.CreatedDate = DateTime.Now;
            entity.LastModified = DateTime.Now;
        }

        protected void PrepareReplaceModel<T>(T entity)
            where T : BaseEntity
        {
            entity.LastModified = DateTime.Now;
        }
    }

    public class BaseService<T> : BaseService, IBaseService<T>
        where T : BaseEntity
    {
        private readonly IRepository<T> _repository;

        protected BaseService(IRepository<T> repository)
        {
            _repository = repository;
        }

        protected IRepository<T> GetRepository()
        {
            return _repository;
        }

        public virtual async Task Add(T entity)
        {
            if (entity == null) return;
            PrepareInsertModel(entity);
            await InitializeInsertModel(entity);
            await _repository.Add(entity);
        }

        public virtual async Task AddMany(IEnumerable<T> entities)
        {
            if (entities == null) return;
            foreach (var entity in entities)
            {
                PrepareInsertModel(entity);
                await InitializeInsertModel(entity);
            }
            await _repository.AddMany(entities);
        }

        public virtual async Task<long> Count(string code)
        {
            return await Count(GetFilterExpression<T>(code));
        }

        public virtual async Task<long> Count(Expression<Func<T, bool>> filter)
        {
            return await Count(new ExpressionContext<T>(filter));
        }

        public virtual async Task<long> Count(FilterDefinition<T> filter)
        {
            return await Count(new ExpressionContext<T>(filter));
        }

        public async Task<long> Count(IExpressionContext<T> context)
        {
            return await _repository.Count(context.GetPreExpression());
        }

        public virtual async Task Delete(string code)
        {
            await _repository.Delete(GetFilterExpression<T>(code));
        }

        public IAggregateFluent<T> GetAggregate()
        {
            return _repository.GetAggregate();
        }

        public virtual async Task<T> GetByCode(string code)
        {
            if (TenjinUtils.IsStringEmpty(code)) return default;
            return await _repository.GetSingleByExpression(GetFilterExpression<T>(code));
        }

        public virtual async Task<IEnumerable<T>> GetByExpression(Expression<Func<T, bool>> filter, SortDefinition<T> sort = null, ProjectionDefinition<T> projection = null)
        {
            return await GetByExpression(new ExpressionContext<T>(filter, sort), projection);
        }

        public virtual async Task<IEnumerable<T>> GetByExpression(FilterDefinition<T> filter, SortDefinition<T> sort = null, ProjectionDefinition<T> projection = null)
        {
            return await GetByExpression(new ExpressionContext<T>(filter, sort), projection);
        }

        public async Task<IEnumerable<T>> GetByExpression(IExpressionContext<T> context, ProjectionDefinition<T> projection = null)
        {
            return await _repository.GetByExpression(context.GetPreExpression(), context.GetSort(), projection);
        }

        public virtual async Task<FetchResult<T>> GetPageByExpression(Expression<Func<T, bool>> filter, int page, int quantity = 10, SortDefinition<T> sort = null, ProjectionDefinition<T> projection = null)
        {
            return await GetPageByExpression(new ExpressionContext<T>(filter, sort), page, quantity, projection);
        }

        public virtual async Task<FetchResult<T>> GetPageByExpression(FilterDefinition<T> filter, int page, int quantity = 10, SortDefinition<T> sort = null, ProjectionDefinition<T> projection = null)
        {
            return await GetPageByExpression(new ExpressionContext<T>(filter, sort), page, quantity, projection);
        }

        public async Task<FetchResult<T>> GetPageByExpression(IExpressionContext<T> context, int page, int quantity = 10, ProjectionDefinition<T> projection = null)
        {
            return new FetchResult<T>
            {
                Count = await _repository.Count(context.GetPreExpression()),
                Models = await _repository.GetPageByExpression(page, quantity > 0 ? quantity : 10, context.GetPreExpression(), context.GetSort(), projection)
            };
        }

        public virtual async Task<T> GetSingleByExpression(Expression<Func<T, bool>> filter, SortDefinition<T> sort = null, ProjectionDefinition<T> projection = null, int index = 1)
        {
            return await GetSingleByExpression(new ExpressionContext<T>(filter, sort), projection, index);
        }

        public virtual async Task<T> GetSingleByExpression(FilterDefinition<T> filter, SortDefinition<T> sort = null, ProjectionDefinition<T> projection = null, int index = 1)
        {
            return await GetSingleByExpression(new ExpressionContext<T>(filter, sort), projection, index);
        }

        public async Task<T> GetSingleByExpression(IExpressionContext<T> context, ProjectionDefinition<T> projection = null, int index = 1)
        {
            return await _repository.GetSingleByExpression(context.GetPreExpression(), context.GetSort(), projection, index);
        }

        public virtual async Task Replace(T entity)
        {
            if (entity == null) return;
            PrepareReplaceModel(entity);
            await InitializeReplaceModel(entity);
            await _repository.Replace(entity);
        }

        public async Task Published(string code)
        {
            await _repository.UpdateOne(GetFilterExpression<T>(code), Builders<T>.Update.Set(x => x.IsPublished, true));
        }

        public async Task Unpublished(string code)
        {
            await _repository.UpdateOne(GetFilterExpression<T>(code), Builders<T>.Update.Set(x => x.IsPublished, false));
        }

        public virtual async Task CreateIndexes()
        {
            var manager = GetRepository().GetCollection().Indexes;
            {
                if (TenjinRefUtils.GenerateIndexes<T>(out var definitions))
                {
                    await manager.CreateOneAsync(CreateIndexModel("_auto", definitions));
                }
            }
            {
                if (TenjinRefUtils.GenerateTextIndexes<T>(out var definitions))
                {
                    await manager.CreateOneAsync(CreateIndexModel("_text", definitions));
                }
            }
            static CreateIndexModel<T> CreateIndexModel(string name, IndexKeysDefinition<T> definition)
            {
                var options = new CreateIndexOptions { Name = name };
                return new CreateIndexModel<T>(definition, options);
            }
        }

        protected virtual async Task InitializeInsertModel(T entity)
        {
            await Task.Yield();
        }

        protected virtual async Task InitializeReplaceModel(T entity)
        {
            await Task.Yield();
        }

    }

    public class BaseService<T, TV> : BaseService, IBaseService<T, TV>
        where T : BaseEntity
        where TV : T
    {
        private readonly IRepository<T> _repository;

        protected BaseService(IRepository<T> repository)
        {
            _repository = repository;
        }

        protected IRepository<T> GetRepository()
        {
            return _repository;
        }

        public virtual async Task Add(T entity)
        {
            if (entity == null) return;
            PrepareInsertModel(entity);
            await InitializeInsertModel(entity);
            await _repository.Add(entity);
        }

        public virtual async Task AddMany(IEnumerable<T> entities)
        {
            if (entities == null) return;
            foreach (var entity in entities)
            {
                PrepareInsertModel(entity);
                await InitializeInsertModel(entity);
            }
            await _repository.AddMany(entities);
        }

        public virtual async Task<long> Count(string code)
        {
            return await _repository.Count(GetFilterExpression<T>(code));
        }

        public virtual async Task<long> Count(Expression<Func<T, bool>> filter)
        {
            return await Count(new ExpressionContext<T, TV>(filter));
        }

        public virtual async Task<long> Count(FilterDefinition<T> filter)
        {
            return await Count(new ExpressionContext<T, TV>(filter));
        }

        public virtual async Task<long> Count(IExpressionContext<T, TV> context)
        {
            var aggregate = CreateViewAggregate(context);
            var counter = await aggregate.Count().FirstOrDefaultAsync();
            return counter?.Count ?? 0;
        }

        public virtual async Task Delete(string code)
        {
            await _repository.Delete(GetFilterExpression<T>(code));
        }

        public IAggregateFluent<T> GetAggregate()
        {
            return _repository.GetAggregate();
        }

        public virtual async Task<TV> GetByCode(string code)
        {
            if (TenjinUtils.IsStringEmpty(code)) return default;
            var aggregate = CreateViewAggregate(new ExpressionContext<T, TV>(GetFilterExpression<T>(code)));
            return await aggregate.FirstOrDefaultAsync();
        }

        public virtual async Task<TV> GetSingleByExpression(Expression<Func<T, bool>> filter, SortDefinition<T> sort = null, ProjectionDefinition<T> projection = null, int index = 1)
        {
            return await GetSingleByExpression(new ExpressionContext<T, TV>(filter, sort), projection, index);
        }

        public virtual async Task<TV> GetSingleByExpression(FilterDefinition<T> filter, SortDefinition<T> sort = null, ProjectionDefinition<T> projection = null, int index = 1)
        {
            return await GetSingleByExpression(new ExpressionContext<T, TV>(filter, sort), projection, index);
        }

        public async Task<TV> GetSingleByExpression(IExpressionContext<T, TV> context, ProjectionDefinition<T> projection = null, int index = 1)
        {
            var aggregate = CreateViewAggregate(context, projection);
            return await aggregate.FirstOrDefaultAsync();
        }

        public virtual async Task<IEnumerable<TV>> GetByExpression(Expression<Func<T, bool>> filter, SortDefinition<T> sort = null, ProjectionDefinition<T> projection = null)
        {
            return await GetByExpression(new ExpressionContext<T, TV>(filter, sort), projection);
        }

        public virtual async Task<IEnumerable<TV>> GetByExpression(FilterDefinition<T> filter, SortDefinition<T> sort = null, ProjectionDefinition<T> projection = null)
        {
            return await GetByExpression(new ExpressionContext<T, TV>(filter, sort), projection);
        }

        public async Task<IEnumerable<TV>> GetByExpression(IExpressionContext<T, TV> context, ProjectionDefinition<T> projection = null)
        {
            var aggregate = CreateViewAggregate(context, projection);
            return await aggregate.ToListAsync();
        }

        public virtual async Task<FetchResult<TV>> GetPageByExpression(Expression<Func<T, bool>> filter, int page, int quantity = 10, SortDefinition<T> sort = null, ProjectionDefinition<T> projection = null)
        {
            return await GetPageByExpression(new ExpressionContext<T, TV>(filter, sort), page, quantity, projection);
        }

        public virtual async Task<FetchResult<TV>> GetPageByExpression(FilterDefinition<T> filter, int page, int quantity = 10, SortDefinition<T> sort = null, ProjectionDefinition<T> projection = null)
        {
            return await GetPageByExpression(new ExpressionContext<T, TV>(filter, sort), page, quantity, projection);
        }

        public async Task<FetchResult<TV>> GetPageByExpression(IExpressionContext<T, TV> context, int page, int quantity = 10, ProjectionDefinition<T> projection = null)
        {
            var aggregate = CreateViewAggregate(context, projection);
            var counter = await aggregate.Count().FirstOrDefaultAsync();
            return  new FetchResult<TV>
            {
                Count = counter?.Count ?? 0,
                Models = await aggregate.Skip((Math.Max(0, page - 1) * quantity)).Limit(quantity > 0 ? quantity : 10).ToListAsync()
            };
        }

        public virtual async Task Replace(T entity)
        {
            if (entity == null) return;
            PrepareReplaceModel(entity);
            await InitializeReplaceModel(entity);
            await _repository.Replace(entity);
        }

        public async Task Published(string code)
        {
            await _repository.UpdateOne(GetFilterExpression<T>(code), Builders<T>.Update.Set(x => x.IsPublished, true));
        }

        public async Task Unpublished(string code)
        {
            await _repository.UpdateOne(GetFilterExpression<T>(code), Builders<T>.Update.Set(x => x.IsPublished, false));
        }

        public virtual async Task CreateIndexes()
        {
            var manager = GetRepository().GetCollection().Indexes;
            {
                if (TenjinRefUtils.GenerateIndexes<T>(out var definitions))
                {
                    await manager.CreateOneAsync(CreateIndexModel("_auto", definitions));
                }
            }
            {
                if (TenjinRefUtils.GenerateTextIndexes<T>(out var definitions))
                {
                    await manager.CreateOneAsync(CreateIndexModel("_text", definitions));
                }
            }
            static CreateIndexModel<T> CreateIndexModel(string name, IndexKeysDefinition<T> definition)
            {
                var options = new CreateIndexOptions { Name = name };
                return new CreateIndexModel<T>(definition, options);
            }
        }

        protected virtual async Task InitializeInsertModel(T entity)
        {
            await Task.Yield();
        }

        protected virtual async Task InitializeReplaceModel(T entity)
        {
            await Task.Yield();
        }

        protected IAggregateFluent<TV> CreateViewAggregate(IExpressionContext<T, TV> context, ProjectionDefinition<T> projection = null)
        {
            var aggregate = GetAggregate();
            if (context.GetPreExpression() != null)
            {
                aggregate = aggregate.Match(context.GetPreExpression());
            }
            if (context.GetSort() != null)
            {
                aggregate = aggregate.Sort(context.GetSort());
            }
            if (projection != null)
            {
                aggregate = aggregate.Project(projection).As<T>();
            }
            return ConvertToViewAggreagate(aggregate, context);
        }

        protected virtual IAggregateFluent<TV> ConvertToViewAggreagate(IAggregateFluent<T> mappings, IExpressionContext<T, TV> context)
        {
            return mappings.As<TV>().Match(context.GetPostExpression());
        }
    }
}
