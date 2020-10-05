using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Tenjin.Apis.Reflections;
using Tenjin.Models.Entities;
using Tenjin.Services.Interfaces;

namespace Tenjin.Apis.Controllers
{
    public abstract class BaseController : TenjinController
    {
        protected virtual SortDefinition<T> GetDefaultSort<T>(IQueryCollection queries) where T : BaseEntity => Builders<T>.Sort.Descending(x => x.CreatedDate);

        protected virtual Expression<Func<T, bool>> GetDefaultExpression<T>(IQueryCollection queries) => x => true;
    }

    public abstract class BaseController<T> : BaseController
        where T : BaseEntity
    {
        private readonly IBaseService<T> _service;

        protected BaseController(IBaseService<T> service)
        {
            _service = service;
        }

        [HttpGet("{page:int}/{quantity:int}")]
        [HttpPost("{page:int}/{quantity:int}")]
        public virtual async Task<IActionResult> GetPageByExpression(int page, int quantity)
        {
            var context = await HttpContextReader.Create(HttpContext);
            return Ok(await _service.GetPageByExpression(context.As<T>(), page, quantity));
        }

        [HttpGet("selectize")]
        public virtual async Task<IActionResult> Selectize()
        {
            return Ok(await _service.GetByExpression(x => x.IsPublished));
        }

        [HttpGet("{code}")]
        public virtual async Task<IActionResult> Get(string code)
        {
            return Ok(await _service.GetByCode(code));
        }

        [HttpGet("count")]
        [HttpPost("count")]
        public virtual async Task<IActionResult> Count()
        {
            var context = await HttpContextReader.Create(HttpContext);
            return Ok(await _service.Count(context.As<T>()));
        }

        [HttpPost]
        public virtual async Task<IActionResult> Post([FromBody] T value)
        {
            await InitializeInsertModel(value);
            await _service.Add(value);
            return Ok(value);
        }

        [HttpPut]
        public virtual async Task<IActionResult> Put([FromBody] T value)
        {
            await InitializeReplaceModel(value);
            await _service.Replace(value);
            return Ok(value);
        }

        [HttpDelete("{code}")]
        public virtual async Task<IActionResult> Delete(string code)
        {
            await _service.Delete(code);
            return Ok();
        }

        [HttpPost("published/{code}")]
        public virtual async Task<IActionResult> Published(string code)
        {
            await _service.Published(code);
            return Ok();
        }

        [HttpPost("unpublished/{code}")]
        public virtual async Task<IActionResult> Unpublished(string code)
        {
            await _service.Unpublished(code);
            return Ok();
        }

        protected virtual async Task InitializeInsertModel(T model)
        {
            await Task.Yield();
        }

        protected virtual async Task InitializeReplaceModel(T model)
        {
            await Task.Yield();
        }
    }

    public abstract class BaseController<T, TV> : BaseController
        where T : BaseEntity
        where TV : T
    {
        private readonly IBaseService<T, TV> _service;

        protected BaseController(IBaseService<T, TV> service)
        {
            _service = service;
        }

        [HttpGet("{page:int}/{quantity:int}")]
        [HttpPost("{page:int}/{quantity:int}")]
        public virtual async Task<IActionResult> GetPageByExpression(int page, int quantity)
        {
            var context = await HttpContextReader.Create(HttpContext);
            return Ok(await _service.GetPageByExpression(context.As<T, TV>(), page, quantity));
        }

        [HttpGet("selectize")]
        public virtual async Task<IActionResult> Selectize()
        {
            return Ok(await _service.GetByExpression(x => x.IsPublished));
        }

        [HttpGet("{code}")]
        public virtual async Task<IActionResult> Get(string code)
        {
            return Ok(await _service.GetByCode(code));
        }

        [HttpGet("count")]
        [HttpPost("count")]
        public virtual async Task<IActionResult> Count()
        {
            var context = await HttpContextReader.Create(HttpContext);
            return Ok(await _service.Count(context.As<T, TV>()));
        }

        [HttpPost]
        public virtual async Task<IActionResult> Post([FromBody] T value)
        {
            await InitializeInsertModel(value);
            await _service.Add(value);
            return Ok(value);
        }

        [HttpPut]
        public virtual async Task<IActionResult> Put([FromBody] T value)
        {
            await InitializeReplaceModel(value);
            await _service.Replace(value);
            return Ok(value);
        }

        [HttpDelete("{code}")]
        public virtual async Task<IActionResult> Delete(string code)
        {
            await _service.Delete(code);
            return Ok();
        }

        [HttpPost("published/{code}")]
        public virtual async Task<IActionResult> Published(string code)
        {
            await _service.Published(code);
            return Ok();
        }

        [HttpPost("unpublished/{code}")]
        public virtual async Task<IActionResult> Unpublished(string code)
        {
            await _service.Unpublished(code);
            return Ok();
        }

        protected virtual async Task InitializeInsertModel(T model)
        {
            await Task.Yield();
        }

        protected virtual async Task InitializeReplaceModel(T model)
        {
            await Task.Yield();
        }
    }
}
