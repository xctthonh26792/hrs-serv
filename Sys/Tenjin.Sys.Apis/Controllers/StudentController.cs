using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tenjin.Apis.Controllers;
using Tenjin.Models;
using Tenjin.Sys.Apis.ExcelReaders;
using Tenjin.Sys.Models.Entities;
using Tenjin.Sys.Models.Views;
using Tenjin.Sys.Services.Interfaces;

namespace Tenjin.Sys.Apis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentController : BaseController<Student, StudentView>
    {
        private readonly IStudentService _service;
        public StudentController(IStudentService service) : base(service)
        {
            _service = service;
        }

        [HttpGet("studentdata")]
        public async Task<IActionResult> GetStudentDataForAction()
        {
            return Ok(await _service.GetStudentDataForAction());
        }

        [HttpPost("import")]
        public async Task<IActionResult> Import(IFormFile file)
        {
            var stream = file.OpenReadStream();
            var reader = new StudentReader(stream);
            var models = reader.Parse();
            var list = new List<StudentBuffer>();
            var data = await _service.GetStudentDataForAction();
            if (!models.Any())
            {
                return BadRequest("File trống");
            }
            foreach (var model in models)
            {
                var row = model.Row;
                var student = model.Model;
                var buffer = new StudentBuffer { Student = new Student(), Errors = new List<string>() };
                if (string.IsNullOrEmpty(student.Code))
                {
                    buffer.Errors.Add("Mã sinh viên trống.");
                    buffer.Index = row;
                }
                else
                {
                    buffer.Student.DefCode = student.Code;
                }
                if (string.IsNullOrEmpty(student.Name))
                {
                    buffer.Errors.Add("Tên sinh viên trống.");
                    buffer.Index = row;
                }
                else
                {
                    buffer.Student.Name = student.Name;
                }
                buffer.Student.DateOfBirth = student.DateOfBirth;
                buffer.Student.Email = student.Email;
                buffer.Student.Phone = student.Phone;
                buffer.Student.CreatedDate = DateTime.Now;
                buffer.Student.LastModified = DateTime.Now;
                buffer.Student.IsPublished = true;
                if (string.IsNullOrEmpty(student.CenterCode))
                {
                    buffer.Errors.Add("Mã trường trống");
                    buffer.Index = row;
                }
                else
                {
                    var center = data?.Centers.ToList()?.FirstOrDefault(x => x.DefCode == student.CenterCode);
                    if (center == null)
                    {
                        buffer.Errors.Add("Không tìm thấy trường trên hệ thống");
                        buffer.Index = row;
                    }
                    else
                    {
                        buffer.Student.CenterCode = center.Id.ToObjectId();
                    }
                }

                if (string.IsNullOrEmpty(student.MajorCode))
                {
                    buffer.Errors.Add("Mã chuyên môn trống");
                    buffer.Index = row;
                }
                else
                {
                    var major = data?.Majors.ToList()?.FirstOrDefault(x => x.DefCode == student.MajorCode);
                    if (major == null)
                    {
                        buffer.Errors.Add("Không tìm thấy chuyên môn trên hệ thống");
                        buffer.Index = row;
                    }
                    else
                    {
                        buffer.Student.MajorCode = major.Id.ToObjectId();
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
            var listToInsert = list.Select(x => x.Student).ToList();
            await _service.Import(listToInsert);
            return Ok();
        }

        public class StudentBuffer
        {
            public int Index { get; set; }

            public List<string> Errors { get; set; }

            public Student Student { get; set; }
        }
    }
}
