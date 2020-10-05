using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Tenjin.Helpers;
using Tenjin.Reflections;
using Tenjin.Services;
using Tenjin.Sys.Contracts.Interfaces;
using Tenjin.Sys.Models.Cores;
using Tenjin.Sys.Models.Entities;
using Tenjin.Sys.Models.Views;
using Tenjin.Sys.Services.Interfaces;

namespace Tenjin.Sys.Services
{
    public class EmployeeService : BaseService<Employee, EmployeeView>, IEmployeeService
    {
        private readonly ISysContext _context;
        private const string TAG = "NV";
        public EmployeeService(ISysContext context) : base(context.EmployeeRepository)
        {
            _context = context;
        }

        public async Task<EmployeeData> GetDataForEmployeeAction()
        {
            return new EmployeeData
            {
                Levels = await FetchLevels(),
                Majors = await FetchMajors(),
                Facutlies = await FetchFacutlies()
            };

            async Task<IEnumerable<Level>> FetchLevels()
            {
                return await _context.LevelRepository.GetByExpression(x => x.IsPublished);
            }

            async Task<IEnumerable<Major>> FetchMajors()
            {
                return await _context.MajorRepository.GetByExpression(x => x.IsPublished);
            }

            async Task<IEnumerable<Facutly>> FetchFacutlies()
            {
                return await _context.FacutlyRepository.GetByExpression(x => x.IsPublished);
            }
        }

        protected override IAggregateFluent<EmployeeView> ConvertToViewAggreagate(IAggregateFluent<Employee> mappings, IExpressionContext<Employee, EmployeeView> context)
        {
            var afState = @"{
                                ""$addFields"": {
                                    ""has_user"": { ""$cond"": [ {  ""$ne"": [""$user"", undefined ] }, true, false ] },
                                    ""username"": ""$user.username"",
                                    ""permission"": { ""$cond"": [ {  ""$ne"": [""$user.permission"", undefined ] }, ""$user.permission"", 0 ] }
                                }
                            }";
            var pjState = @"{
                                ""user"": 0
                            }";
            var unwind = new AggregateUnwindOptions<BsonDocument> { PreserveNullAndEmptyArrays = true };
            return mappings
                .Lookup("user", "_id", "code", "user").Unwind("user", unwind)
                .Lookup("facutly", "facutly_code", "_id", "facutly").Unwind("facutly", unwind)
                .Lookup("level", "level_code", "_id", "level").Unwind("level", unwind)
                .Lookup("major", "major_code", "_id", "major").Unwind("major", unwind)
                .AppendStage<BsonDocument>(BsonDocument.Parse(afState))
                .Project(BsonDocument.Parse(pjState))
                .As<EmployeeView>()
                .Match(context.GetPostExpression());
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

        protected override async Task InitializeInsertModel(Employee entity)
        {
            await base.InitializeInsertModel(entity);
            if (entity != null)
            {
                var parts = entity.Name?.Split(' ');
                entity.FirstName = parts.LastOrDefault()?.ViToEn() ?? string.Empty;
            }
            entity.DefCode = string.IsNullOrEmpty(entity.DefCode) ? await GenerateCode() : entity.DefCode;
            entity.IsPublished = true;
        }

        protected override async Task InitializeReplaceModel(Employee entity)
        {
            await base.InitializeReplaceModel(entity);
            if (entity != null)
            {
                var parts = entity.Name?.Split(' ');
                entity.FirstName = parts.LastOrDefault()?.ViToEn() ?? string.Empty;
            }
            entity.DefCode = string.IsNullOrEmpty(entity.DefCode) ? await GenerateCode() : entity.DefCode;
        }

        public async Task Import(IEnumerable<Employee> entities)
        {
            var models = new List<WriteModel<Employee>>();
            if(entities != null && entities.Any())
            {
                foreach(var entity in entities)
                {
                    await InitializeInsertModel(entity);
                    Expression<Func<Employee, bool>> filter = x => x.DefCode == entity.DefCode;
                    var updater = Builders<Employee>.Update
                        .Set(x => x.Name, entity.Name)
                        .Set(x => x.DateOfBirth, entity.DateOfBirth)
                        .Set(x => x.Email, entity.Email)
                        .Set(x => x.Phone, entity.Phone)
                        .Set(x => x.LevelCode, entity.LevelCode)
                        .Set(x => x.MajorCode, entity.MajorCode)
                        .Set(x => x.FacutlyCode, entity.FacutlyCode)
                        .Set(x => x.FirstName, entity.FirstName)
                        .Set(x => x.ValueToSearch, entity.ValueToSearch)
                        .Set(x => x.CreatedDate, entity.CreatedDate)
                        .Set(x => x.IsPublished, entity.IsPublished)
                        .Set(x => x.LastModified, entity.LastModified);
                        
                    var model = new UpdateOneModel<Employee>(filter, updater)
                    {
                        IsUpsert = true
                    };
                    models.Add(model);
                    if (models.Count >= 1000)
                    {
                        await _context.EmployeeRepository.BulkWrite(models);
                        models.Clear();
                    }
                }
                if(models.Count > 0)
                {
                    await _context.EmployeeRepository.BulkWrite(models);
                    models.Clear();
                }
            }
        }
    }
}
