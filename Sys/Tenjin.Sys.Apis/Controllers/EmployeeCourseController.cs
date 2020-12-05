using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tenjin.Apis.Controllers;
using Tenjin.Helpers;
using Tenjin.Models;
using Tenjin.Sys.Apis.ExcelReaders;
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
        [HttpPost("import")]
        public async Task<IActionResult> Import(IFormFile file)
        {
            var stream = file.OpenReadStream();
            var reader = new EmployeeCourseReader(stream);
            var models = reader.Parse();
            var list = new List<EmployeeCourseBuffer>();
            var data = await _service.GetDataForEmployeeCourseAction();
            if (!models.Any())
            {
                return BadRequest("File trống");
            }
            foreach (var model in models)
            {
                var row = model.Row;
                var ecourse = model.Model;
                var buffer = new EmployeeCourseBuffer { EmployeeCourse = new EmployeeCourse(), Errors = new List<string>() };
                if (string.IsNullOrEmpty(ecourse.Code))
                {
                    buffer.Errors.Add("Mã trống.");
                    buffer.Index = row;
                }
                else
                {
                    buffer.EmployeeCourse.DefCode = ecourse.Code;
                }

                if (string.IsNullOrEmpty(ecourse.EmployeeCode))
                {
                    buffer.Errors.Add("Mã nhân viên trống");
                    buffer.Index = row;
                }
                else
                {
                    var employee = data?.Employees?.FirstOrDefault(x => x.DefCode == ecourse.EmployeeCode);
                    if (employee == null)
                    {
                        buffer.Errors.Add("Không tìm thấy nhân viên trên hệ thống.");
                        buffer.Index = row;
                    }
                    else
                    {
                        buffer.EmployeeCourse.EmployeeCode = employee.Id.ToObjectId();
                        buffer.EmployeeCourse.MajorCode = employee.MajorCode;
                        buffer.EmployeeCourse.FacutlyCode = employee.FacutlyCode;
                    }
                }
                buffer.EmployeeCourse.CreatedDate = DateTime.Now;
                buffer.EmployeeCourse.LastModified = DateTime.Now;
                buffer.EmployeeCourse.IsPublished = true;
                if (string.IsNullOrEmpty(ecourse.CourseCode))
                {
                    buffer.Errors.Add("Mã khóa học trống");
                    buffer.Index = row;
                }
                else
                {
                    var course = data.Courses.ToList()?.FirstOrDefault(x => x.DefCode == ecourse.CourseCode);
                    if (course == null)
                    {
                        buffer.Errors.Add("Không tìm thấy khóa học trên hệ thống");
                        buffer.Index = row;
                    }
                    else
                    {
                        buffer.EmployeeCourse.CourseCode = course.Id.ToObjectId();
                        buffer.EmployeeCourse.Start = course.Start;
                        buffer.EmployeeCourse.End = course.End;
                        buffer.EmployeeCourse.TimeStart = course.TimeStart;
                        buffer.EmployeeCourse.TimeEnd = course.TimeEnd;
                        buffer.EmployeeCourse.CourseTime = course.CourseTime;
                    }
                }
                
                list.Add(buffer);
            }

            if (list.Any(x => x.Errors.Any()))
            {
                return BadRequest(new
                {
                    Errors = list.Where(x => x.Errors.Any()).ToList()
                });
            }
            var listToInsert = list.Select(x => x.EmployeeCourse).ToList();
            await _service.Import(listToInsert);
            return Ok();
        }
    }

    public class EmployeeCourseBuffer
    {
        public List<string> Errors { get; set; }

        public int Index { get; set; }

        public EmployeeCourse EmployeeCourse { get; set; }
    }

}
