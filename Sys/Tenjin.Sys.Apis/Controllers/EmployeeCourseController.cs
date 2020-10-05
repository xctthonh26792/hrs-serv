using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Tenjin.Apis.Controllers;
using Tenjin.Sys.Models.Entities;
using Tenjin.Sys.Models.Views;
using Tenjin.Sys.Services.Interfaces;

namespace Tenjin.Sys.Apis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeCourseController : BaseController<EmployeeCourse, EmployeeCourseView>
    {
        private readonly IEmployeeCourseService _service;
        public EmployeeCourseController(IEmployeeCourseService service) : base(service)
        {
            _service = service;
        }

        [HttpGet("employeecoursedata")]
        public async Task<IActionResult> GetDataForIntershipAction()
        {
            return Ok(await _service.GetDataForEmployeeCourseAction());
        }

        public override async Task<IActionResult> Post([FromBody] EmployeeCourse value)
        {
            var isExist = await _service.Validate(value);
            await InitializeInsertModel(value);
            if (isExist)
            {
                return BadRequest();
            }
            await base.Post(value);
            return Ok();
        }

        public override async Task<IActionResult> Put([FromBody] EmployeeCourse value)
        {
            var isExist = await _service.Validate(value);
            if (isExist)
            {
                return BadRequest();
            }
            await InitializeReplaceModel(value);
            await base.Put(value);
            return Ok();
        }

        protected override async Task InitializeInsertModel(EmployeeCourse model)
        {
            model.CreatedDate = DateTime.Now;
            model.IsPublished = true;
            await base.InitializeInsertModel(model);
        }

        protected override async Task InitializeReplaceModel(EmployeeCourse model)
        {
            model.LastModified = DateTime.Now;
            await base.InitializeReplaceModel(model);
        }
    }
}
