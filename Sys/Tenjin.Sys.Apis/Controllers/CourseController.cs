using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Tenjin.Apis.Controllers;
using Tenjin.Helpers;
using Tenjin.Sys.Models.Entities;
using Tenjin.Sys.Services.Interfaces;

namespace Tenjin.Sys.Apis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CourseController : BaseController<Course>
    {
        private ICourseService _service;
        public CourseController(ICourseService service) : base(service)
        {
            _service = service;
        }

        [HttpPost("all-course")]
        public async Task<IActionResult> GetAllCourse([FromBody] CourseModel model)
        {
            var query = model.Query?.ToSeoUrl();
            Expression<Func<Course, bool>> filter = x => x.IsPublished == true;
            filter = string.IsNullOrEmpty(query) ? filter : filter.And(x => x.ValueToSearch.Contains(query));
            return Ok(await _service.GetPageByExpression(filter, model.Page, model.Quantity));
        }


        public class CourseModel
        {
            public string Query { get; set; }

            public int Page { get; set; } = 1;

            public int Quantity { get; set; } = 9;
        }
    }
}
