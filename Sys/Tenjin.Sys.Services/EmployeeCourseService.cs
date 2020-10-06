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
    public class EmployeeCourseService : BaseService<EmployeeCourse, EmployeeCourseView>, IEmployeeCourseService
    {
        private readonly ISysContext _context;
        private const string TAG = "KH";
        public EmployeeCourseService(ISysContext context) : base(context.EmployeeCourseRepository)
        {
            _context = context;
        }

        protected override IAggregateFluent<EmployeeCourseView> ConvertToViewAggreagate(IAggregateFluent<EmployeeCourse> mappings, IExpressionContext<EmployeeCourse, EmployeeCourseView> context)
        {
            var unwind = new AggregateUnwindOptions<BsonDocument> { PreserveNullAndEmptyArrays = true };
            return mappings.Lookup("employee", "employee_code", "_id", "employee").Unwind("employee", unwind)
                .Lookup("course", "course_code", "_id", "course").Unwind("course", unwind)
                .Lookup("facutly", "facutly_code", "_id", "facutly").Unwind("facutly", unwind)
                .Lookup("major", "major_code", "_id", "major").Unwind("major", unwind)
                .As<EmployeeCourseView>().Match(context.GetPostExpression());
        }

        public async Task<bool> Validate(EmployeeCourse entity)
        {
            var from = $"{entity.Start} {entity.TimeStart}";
            var to = $"{entity.End} {entity.TimeEnd}";
            if (string.IsNullOrEmpty(entity.Id))
            {
                var expression =
                    Builders<EmployeeCourse>.Filter.And(
                        Builders<EmployeeCourse>.Filter.Eq(x => x.IsPublished, true),
                        Builders<EmployeeCourse>.Filter.Eq(x => x.EmployeeCode, entity.EmployeeCode),
                        Builders<EmployeeCourse>.Filter.Or(
                            Builders<EmployeeCourse>.Filter.And(
                                Builders<EmployeeCourse>.Filter.Lte(x => x.CourseStart, from),
                                Builders<EmployeeCourse>.Filter.Gte(x => x.CourseEnd, from)
                            ),
                            Builders<EmployeeCourse>.Filter.And(
                                Builders<EmployeeCourse>.Filter.Lte(x => x.CourseStart, from),
                                Builders<EmployeeCourse>.Filter.Gte(x => x.CourseEnd, to)
                            ),
                            Builders<EmployeeCourse>.Filter.And(
                                Builders<EmployeeCourse>.Filter.Lte(x => x.CourseStart, from),
                                Builders<EmployeeCourse>.Filter.Gte(x => x.CourseEnd, to)
                            )
                    )
                );
                return await _context.EmployeeCourseRepository.Count(expression) > 0;
            }
            else
            {
                var expression =
                   Builders<EmployeeCourse>.Filter.And(
                       Builders<EmployeeCourse>.Filter.Ne(x => x.Id, entity.Id),
                       Builders<EmployeeCourse>.Filter.Eq(x => x.IsPublished, true),
                       Builders<EmployeeCourse>.Filter.Eq(x => x.EmployeeCode, entity.EmployeeCode),
                       Builders<EmployeeCourse>.Filter.Or(
                           Builders<EmployeeCourse>.Filter.And(
                                Builders<EmployeeCourse>.Filter.Lte(x => x.CourseStart, from),
                                Builders<EmployeeCourse>.Filter.Gte(x => x.CourseEnd, from)
                            ),
                            Builders<EmployeeCourse>.Filter.And(
                                Builders<EmployeeCourse>.Filter.Lte(x => x.CourseStart, from),
                                Builders<EmployeeCourse>.Filter.Gte(x => x.CourseEnd, to)
                            ),
                            Builders<EmployeeCourse>.Filter.And(
                                Builders<EmployeeCourse>.Filter.Lte(x => x.CourseStart, from),
                                Builders<EmployeeCourse>.Filter.Gte(x => x.CourseEnd, to)
                            )
                   )
               );
                return await _context.EmployeeCourseRepository.Count(expression) > 0;
            }
        }

        public async Task<EmployeeCourseData> GetDataForEmployeeCourseAction()
        {
            return new EmployeeCourseData
            {
                Employees = await FetchEmployee(),
                Courses = await FetchCourse()
            };
            async Task<IEnumerable<Employee>> FetchEmployee()
            {
                return await _context.EmployeeRepository.GetByExpression(x => x.IsPublished);
            }

            async Task<IEnumerable<Course>> FetchCourse()
            {
                return await _context.CourseRepository.GetByExpression(x => x.IsPublished);
            }
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

        protected override async Task InitializeInsertModel(EmployeeCourse entity)
        {
            if (entity == null)
            {
                return;
            }
            await base.InitializeInsertModel(entity);
            entity.IsPublished = true;
            entity.DefCode = string.IsNullOrEmpty(entity.DefCode) ? await GenerateCode() : entity.DefCode;
        }

        public async Task Import(IEnumerable<EmployeeCourse> entities)
        {
            var models = new List<WriteModel<EmployeeCourse>>();
            if (entities != null && entities.Any())
            {
                foreach (var entity in entities)
                {
                    await InitializeInsertModel(entity);
                    Expression<Func<EmployeeCourse, bool>> filter = x => x.DefCode == entity.DefCode;
                    var updater = Builders<EmployeeCourse>.Update
                        .Set(x => x.EmployeeCode, entity.EmployeeCode)
                        .Set(x => x.FacutlyCode, entity.FacutlyCode)
                        .Set(x => x.CourseCode, entity.CourseCode)
                        .Set(x => x.MajorCode, entity.MajorCode)
                        .Set(x => x.TimeStart, entity.TimeStart)
                        .Set(x => x.TimeEnd, entity.TimeEnd)
                        .Set(x => x.Start, entity.Start)
                        .Set(x => x.End, entity.End)
                        .Set(x => x.CourseStart, entity.CourseStart)
                        .Set(x => x.CourseEnd, entity.CourseEnd)
                        .Set(x => x.MajorCode, entity.MajorCode)
                        .Set(x => x.ValueToSearch, entity.ValueToSearch)
                        .Set(x => x.CreatedDate, entity.CreatedDate)
                        .Set(x => x.IsPublished, entity.IsPublished)
                        .Set(x => x.LastModified, entity.LastModified);

                    var model = new UpdateOneModel<EmployeeCourse>(filter, updater)
                    {
                        IsUpsert = true
                    };
                    models.Add(model);
                    if (models.Count >= 1000)
                    {
                        await _context.EmployeeCourseRepository.BulkWrite(models);
                        models.Clear();
                    }
                }
                if (models.Count > 0)
                {
                    await _context.EmployeeCourseRepository.BulkWrite(models);
                    models.Clear();
                }
            }
        }
    }
}
