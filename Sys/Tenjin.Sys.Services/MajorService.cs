﻿using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tenjin.Services;
using Tenjin.Sys.Contracts.Interfaces;
using Tenjin.Sys.Models.Entities;
using Tenjin.Sys.Services.Interfaces;

namespace Tenjin.Sys.Services
{
    public class MajorService : BaseService<Major>, IMajorService
    {
        private readonly ISysContext _context;
        private const string TAG = "CM";
        public MajorService(ISysContext context) : base(context.MajorRepository)
        {
            _context = context;
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

        protected override async Task InitializeInsertModel(Major entity)
        {
            if (entity == null)
            {
                return;
            }
            await base.InitializeInsertModel(entity);
            entity.IsPublished = true;
            entity.DefCode = string.IsNullOrEmpty(entity.DefCode) ? await GenerateCode() : entity.DefCode;
        }
    }
}
