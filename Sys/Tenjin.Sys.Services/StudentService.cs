﻿using MongoDB.Bson;
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
    public class StudentService : BaseService<Student, StudentView>, IStudentService
    {
        private readonly ISysContext _context;
        private const string TAG = "SV";
        public StudentService(ISysContext context) : base(context.StudentRepository)
        {
            _context = context;
        }
        
        protected override IAggregateFluent<StudentView> ConvertToViewAggreagate(IAggregateFluent<Student> mappings, IExpressionContext<Student, StudentView> context)
        {
            var unwind = new AggregateUnwindOptions<BsonDocument> { PreserveNullAndEmptyArrays = true };
            return mappings.Lookup("center", "center_code", "_id", "center").Unwind("center", unwind)
                .Lookup("major", "major_code", "_id", "major").Unwind("major", unwind)
                .Lookup("classroom", "classroom_code", "_id", "classroom").Unwind("classroom", unwind)
                .As<StudentView>().Match(context.GetPostExpression());
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

        protected override async Task InitializeInsertModel(Student entity)
        {
            if (entity == null)
            {
                return;
            }
            await base.InitializeInsertModel(entity);
            entity.IsPublished = true;
            entity.DefCode = string.IsNullOrEmpty(entity.DefCode) ? await GenerateCode() : entity.DefCode;
        }

        public async Task Import(IEnumerable<Student> entities)
        {
            var models = new List<WriteModel<Student>>();
            if(entities != null && entities.Any())
            {
                foreach (var entity in entities)
                {
                    await InitializeInsertModel(entity);
                    Expression<Func<Student, bool>> filter = x => x.DefCode == entity.DefCode;
                    var updater = Builders<Student>.Update
                        .Set(x => x.Name, entity.Name)
                        .Set(x => x.DateOfBirth, entity.DateOfBirth)
                        .Set(x => x.Email, entity.Email)
                        .Set(x => x.Phone, entity.Phone)
                        .Set(x => x.Course, entity.Course)
                        .Set(x => x.MajorCode, entity.MajorCode)
                        .Set(x => x.ClassroomCode, entity.ClassroomCode)
                        .Set(x => x.Class, entity.Class)
                        .Set(x => x.CenterCode, entity.CenterCode)
                        .Set(x => x.ValueToSearch, entity.ValueToSearch)
                        .Set(x => x.CreatedDate, entity.CreatedDate)
                        .Set(x => x.IsPublished, entity.IsPublished)
                        .Set(x => x.LastModified, entity.LastModified);

                    var model = new UpdateOneModel<Student>(filter, updater)
                    {
                        IsUpsert = true
                    };
                    models.Add(model);
                    if (models.Count >= 1000)
                    {
                        await _context.StudentRepository.BulkWrite(models);
                        models.Clear();
                    }
                }
                if (models.Count > 0)
                {
                    await _context.StudentRepository.BulkWrite(models);
                    models.Clear();
                }
            }
        }

        public async Task<StudentData> GetStudentDataForAction()
        {
            return new StudentData
            {
                Centers = await FetchCenter(),
                Majors = await FetchMajor(),
                Classrooms = await FetchClassroom()
            };

            async Task<IEnumerable<Center>> FetchCenter()
            {
                return await _context.CenterRepository.GetByExpression(x => x.IsPublished);
            }

            async Task<IEnumerable<Major>> FetchMajor()
            {
                return await _context.MajorRepository.GetByExpression(x => x.IsPublished);
            }

            async Task<IEnumerable<Classroom>> FetchClassroom()
            {
                return await _context.ClassroomRepository.GetByExpression(x => x.IsPublished);
            }
        }
    }
}
