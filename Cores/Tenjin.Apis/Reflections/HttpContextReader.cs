using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Tenjin.Apis.Reflections.Interfaces;
using Tenjin.Models.Entities;
using Tenjin.Reflections;

namespace Tenjin.Apis.Reflections
{
    public class HttpContextReader : IHttpContextReader
    {
        private readonly HttpContext _context;
        private IHttpExpressionReader _reader;
        private HttpContextReader(HttpContext context)
        {
            _context = context;
        }

        public static async Task<IHttpContextReader> Create(HttpContext context)
        {
            return await new HttpContextReader(context).Configture();
        }

        public IExpressionContext<T> As<T>()
            where T : BaseEntity
        {
            return new HttpExpressionContext<T>(_reader);
        }

        public IExpressionContext<T, TV> As<T, TV>()
            where T : BaseEntity
            where TV : T
        {
            return new HttpExpressionContext<T, TV>(_reader);
        }

        private async Task<IHttpContextReader> Configture()
        {
            _reader = await HttpExpressionReader.Create(_context);
            return this;
        }
    }
}
