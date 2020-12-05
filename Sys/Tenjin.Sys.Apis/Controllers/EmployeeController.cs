using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class EmployeeController : BaseController<Employee, EmployeeView>
    {
        private readonly IEmployeeService _service;
        public EmployeeController(IEmployeeService service) : base(service)
        {
            _service = service;
        }

        [HttpGet("employeedata")]
        public async Task<IActionResult> GetEmployeeDataForAction()
        {
            return Ok(await _service.GetDataForEmployeeAction());
        }

        [HttpPost("import")]
        public async Task<IActionResult> Import(IFormFile file)
        {
            var stream = file.OpenReadStream();
            var reader = new EmployeeReader(stream);
            var models = reader.Parse();
            var list = new List<EmployeeBuffer>();
            var data = await _service.GetDataForEmployeeAction();
            if(!models.Any())
            {
                return BadRequest("File trống");
            }
            foreach(var model in models)
            {
                var row = model.Row;
                var staff = model.Model;
                var buffer = new EmployeeBuffer { Employee = new Employee(), Errors = new List<string>() };
                if(string.IsNullOrEmpty(staff.Code))
                {
                    buffer.Errors.Add("Mã nhân viên trống.");
                    buffer.Index = row;
                } else
                {
                    buffer.Employee.DefCode = staff.Code;
                }
                if (string.IsNullOrEmpty(staff.Name))
                {
                    buffer.Errors.Add("Tên nhân viên trống.");
                    buffer.Index = row;
                }
                else
                {
                    buffer.Employee.Name = staff.Name;
                }
                buffer.Employee.DateOfBirth = staff.DateOfBirth;
                buffer.Employee.Email = staff.Email;
                buffer.Employee.Phone = staff.Phone;
                buffer.Employee.CreatedDate = DateTime.Now;
                buffer.Employee.LastModified = DateTime.Now;
                buffer.Employee.IsPublished = true;
                if(string.IsNullOrEmpty(staff?.FacutlyCode.ToString()))
                {
                    buffer.Errors.Add("Mã khoa trống");
                    buffer.Index = row;
                } else
                {
                    var facutly = data.Facutlies.ToList()?.FirstOrDefault(x => x.DefCode == staff.FacutlyCode);
                    if (facutly == null)
                    {
                        buffer.Errors.Add("Không tìm thấy khoa trên hệ thống");
                        buffer.Index = row;
                    }
                    else
                    {
                        buffer.Employee.FacutlyCode = facutly.Id.ToObjectId();
                    }
                }

                if (string.IsNullOrEmpty(staff.MajorCode.ToString()))
                {
                    buffer.Errors.Add("Mã chuyên môn trống");
                    buffer.Index = row;
                }
                else
                {
                    var major = data.Majors.ToList()?.FirstOrDefault(x => x.DefCode == staff.MajorCode);
                    if (major == null)
                    {
                        buffer.Errors.Add("Không tìm thấy chuyên môn trên hệ thống");
                        buffer.Index = row;
                    }
                    else
                    {
                        buffer.Employee.MajorCode = major.Id.ToObjectId();
                    }
                }

                if (string.IsNullOrEmpty(staff.LevelCode.ToString()))
                {
                    buffer.Errors.Add("Mã trình độ trống");
                    buffer.Index = row;
                }
                else
                {
                    var level = data.Levels.ToList()?.FirstOrDefault(x => x.DefCode == staff.LevelCode);
                    if (level == null)
                    {
                        buffer.Errors.Add("Không tìm thấy trình độ trên hệ thống");
                        buffer.Index = row;
                    }
                    else
                    {
                        buffer.Employee.LevelCode = level.Id.ToObjectId();
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
            var listToInsert = list.Select(x => x.Employee).ToList();
            await _service.Import(listToInsert);
            return Ok();
        }

        public class EmployeeBuffer
        {
            public List<string> Errors { get; set; }

            public int Index { get; set; }

            public Employee Employee { get; set; }
        }
    }
}
