using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tenjin.Apis.Extensions;
using Tenjin.Apis.Reflections.Interfaces;
using Tenjin.Apis.Reflections.Models;
using Tenjin.Helpers;
using Tenjin.Models.Entities;

namespace Tenjin.Apis.Reflections
{
    public class HttpExpressionReader : IHttpExpressionReader
    {
        private readonly HttpContext _context;
        private readonly string _keyword;
        private readonly string _sort_name;
        private readonly string _sort_direction;
        private readonly List<HttpExpressionDefinition> _definitions;
        private HttpExpressionReader(HttpContext context)
        {
            _context = context;
            _sort_name = _context.Request.Query.TryGetSingle("sort_name", "CreatedDate");
            _sort_direction = _context.Request.Query.TryGetSingle("sort_direction", "desc");
            _keyword = _context.Request.Query.TryGetSingle("keyword", string.Empty);
            _definitions = new List<HttpExpressionDefinition>();
        }

        public static async Task<IHttpExpressionReader> Create(HttpContext context)
        {
            return await new HttpExpressionReader(context).Configture();
        }

        private async Task<HttpExpressionReader> Configture()
        {
            if (_context.Request.Method == HttpMethods.Post)
            {
                var text = await new StreamReader(_context.Request.Body).ReadToEndAsync();
                var filters = text.TryParseJson<JArray>();
                foreach (var filter in filters)
                {
                    var definition = new HttpExpressionDefinition(filter);
                    if (definition.IsCorrect)
                    {
                        _definitions.Add(definition);
                    }
                }
            }
            return this;
        }

        public FilterDefinition<T> GetPreExpression<T>()
            where T : BaseEntity
        {
            var expression = Builders<T>.Filter.Where(x => true);
            if (TenjinUtils.IsStringNotEmpty(_keyword))
            {
                if (_keyword.Contains("*") && TenjinRefUtils.GenerateTextIndexes<T>(out var _))
                {
                    expression = Builders<T>.Filter.And(expression, Builders<T>.Filter.Text(_keyword.Replace("*", " ").Trim()));
                }
                else
                {
                    var patterns = Regex.Matches(_keyword, "([a-zA-Z0-9]*)")
                        .Select(x => x.Value?.Trim()).Where(x => !string.IsNullOrEmpty(x))
                        .Select(x => Builders<T>.Filter.Regex("ValueToSearch", new BsonRegularExpression(x)));
                    expression = Builders<T>.Filter.And(expression, Builders<T>.Filter.Or(patterns));
                }
            }
            return Builders<T>.Filter.And(expression.Concat(_definitions.Where(x => x.GetStage() == HttpExpressionDefinition.PRE_STAGE).Select(x => x.GetDefinition<T>())));
        }

        public FilterDefinition<T> GetPostExpression<T>()
            where T : BaseEntity
        {
            var defs = _definitions
                        .Where(x => x.GetStage() == HttpExpressionDefinition.POST_STAGE)
                        .Select(x => x.GetDefinition<T>());
            return defs.Any() ? Builders<T>.Filter.And(defs) : Builders<T>.Filter.Where(x => true);
        }

        public SortDefinition<T> GetSort<T>()
            where T : BaseEntity
        {
            return (_sort_direction == "desc") ? Builders<T>.Sort.Descending(_sort_name) : Builders<T>.Sort.Ascending(_sort_name);
        }
    }

}
