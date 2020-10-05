using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Tenjin.Reflections;
using Tenjin.Services;
using Tenjin.Sys.Contracts.Interfaces;
using Tenjin.Sys.Models.Cores;
using Tenjin.Sys.Models.Entities;
using Tenjin.Sys.Models.Views;
using Tenjin.Sys.Services.Interfaces;

namespace Tenjin.Sys.Services
{
    public class IntershipService : BaseService<Intership, IntershipView>, IIntershipService
    {
        private readonly ISysContext _context;
        private const string TAG = "TT";
        public IntershipService(ISysContext context) : base(context.IntershipRepository)
        {
            _context = context;
        }

        public async Task<IntershipData> GetDataForIntershipAction()
        {
            return new IntershipData
            {
                Students = await FetchStudent(),
                Facutlies = await FetchFacutly()
            };
            async Task<IEnumerable<Student>> FetchStudent()
            {
                return await _context.StudentRepository.GetByExpression(x => x.IsPublished);
            }

            async Task<IEnumerable<Facutly>> FetchFacutly()
            {
                return await _context.FacutlyRepository.GetByExpression(x => x.IsPublished);
            }
        }
        
        public async Task<bool> Validate(Intership entity)
        {
            if (string.IsNullOrEmpty(entity.Id))
            {
                var expression =
                    Builders<Intership>.Filter.And(
                        Builders<Intership>.Filter.Eq(x => x.IsPublished, true),
                        Builders<Intership>.Filter.Eq(x => x.StudentCode, entity.StudentCode),
                        Builders<Intership>.Filter.Or(
                            Builders<Intership>.Filter.And(
                                Builders<Intership>.Filter.Lte(x => x.Start, entity.Start),
                                Builders<Intership>.Filter.Gte(x => x.End, entity.Start)
                            ),
                            Builders<Intership>.Filter.And(
                                Builders<Intership>.Filter.Lte(x => x.Start, entity.Start),
                                Builders<Intership>.Filter.Gte(x => x.End, entity.End)
                            ),
                            Builders<Intership>.Filter.And(
                                Builders<Intership>.Filter.Lte(x => x.Start, entity.End),
                                Builders<Intership>.Filter.Gte(x => x.End, entity.End)
                            )
                    )
                );
                return await _context.IntershipRepository.Count(expression) > 0;
            }
            else
            {
                var expression =
                   Builders<Intership>.Filter.And(
                       Builders<Intership>.Filter.Ne(x => x.Id, entity.Id),
                       Builders<Intership>.Filter.Eq(x => x.IsPublished, true),
                       Builders<Intership>.Filter.Eq(x => x.StudentCode, entity.StudentCode),
                       Builders<Intership>.Filter.Or(
                           Builders<Intership>.Filter.And(
                                Builders<Intership>.Filter.Lte(x => x.Start, entity.Start),
                                Builders<Intership>.Filter.Gte(x => x.End, entity.Start)
                            ),
                            Builders<Intership>.Filter.And(
                                Builders<Intership>.Filter.Lte(x => x.Start, entity.Start),
                                Builders<Intership>.Filter.Gte(x => x.End, entity.End)
                            ),
                            Builders<Intership>.Filter.And(
                                Builders<Intership>.Filter.Lte(x => x.Start, entity.End),
                                Builders<Intership>.Filter.Gte(x => x.End, entity.End)
                            )
                   )
               );
                return await _context.IntershipRepository.Count(expression) > 0;
            }
        }

        protected override IAggregateFluent<IntershipView> ConvertToViewAggreagate(IAggregateFluent<Intership> mappings, IExpressionContext<Intership, IntershipView> context)
        {
            var unwind = new AggregateUnwindOptions<BsonDocument> { PreserveNullAndEmptyArrays = true };
            return mappings.Lookup("student", "student_code", "_id", "student").Unwind("student", unwind)
                .Lookup("facutly", "facutly_code", "_id", "facutly").Unwind("facutly", unwind)
                .Lookup("center", "center_code", "_id", "center").Unwind("center", unwind)
                .Lookup("major", "major_code", "_id", "major").Unwind("major", unwind)
                .As<IntershipView>().Match(context.GetPostExpression());
        }

        public async Task<string> GenerateCode()
        {
            var today = DateTime.Now.ToString("yyMM");
            var code = $"{TAG}-{today}";
            var filter = Builders<CodeGenerate>.Filter.Where(x => x.Code == code);
            var updater = Builders<CodeGenerate>.Update
                .SetOnInsert(x => x.Code, code)
                .SetOnInsert(x => x.CreatedDate, DateTime.Now)
                .Set(x => x.LastModified, DateTime.Now)
                .Set(x => x.IsPublished, true)
                .Inc(x => x.Count, 1);
            var options = new FindOneAndUpdateOptions<CodeGenerate, CodeGenerate>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };
            var model = await _context.CodeGenerateRepository.GetCollection().FindOneAndUpdateAsync(filter, updater, options);
            return $"{TAG}{today}{model.Count:D3}";
        }

        protected override async Task InitializeInsertModel(Intership entity)
        {
            if (entity == null)
            {
                return;
            }
            await base.InitializeInsertModel(entity);
            entity.IsPublished = true;
            entity.DefCode = string.IsNullOrEmpty(entity.DefCode) ? await GenerateCode() : entity.DefCode;
        }

        public async Task Import(IEnumerable<Intership> entities)
        {
            var models = new List<WriteModel<Intership>>();
            if (entities != null && entities.Any())
            {
                foreach (var entity in entities)
                {
                    await InitializeInsertModel(entity);
                    Expression<Func<Intership, bool>> filter = x => x.DefCode == entity.DefCode;
                    var updater = Builders<Intership>.Update
                        .Set(x => x.StudentCode, entity.StudentCode)
                        .Set(x => x.FacutlyCode, entity.FacutlyCode)
                        .Set(x => x.CenterCode, entity.CenterCode)
                        .Set(x => x.Course, entity.Course)
                        .Set(x => x.MajorCode, entity.MajorCode)
                        .Set(x => x.Class, entity.Class)
                        .Set(x => x.Start, entity.Start)
                        .Set(x => x.End, entity.End)
                        .Set(x => x.ValueToSearch, entity.ValueToSearch)
                        .Set(x => x.CreatedDate, entity.CreatedDate)
                        .Set(x => x.IsPublished, entity.IsPublished)
                        .Set(x => x.LastModified, entity.LastModified);

                    var model = new UpdateOneModel<Intership>(filter, updater)
                    {
                        IsUpsert = true
                    };
                    models.Add(model);
                    if (models.Count >= 1000)
                    {
                        await _context.IntershipRepository.BulkWrite(models);
                        models.Clear();
                    }
                }
                if (models.Count > 0)
                {
                    await _context.IntershipRepository.BulkWrite(models);
                    models.Clear();
                }
            }
        }
    }
}
