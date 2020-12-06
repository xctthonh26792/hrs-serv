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
    public class IntershipController : BaseController<Intership, IntershipView>
    {
        private readonly IIntershipService _service;
        public IntershipController(IIntershipService service) : base(service)
        {
            _service = service;
        }

        [HttpGet("intershipdata")]
        public async Task<IActionResult> GetDataForIntershipAction()
        {
            return Ok(await _service.GetDataForIntershipAction());
        }


        public override async Task<IActionResult> Post([FromBody] Intership value)
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

        public override async Task<IActionResult> Put([FromBody] Intership value)
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

        protected override async Task InitializeInsertModel(Intership model)
        {
            model.CreatedDate = DateTime.Now;
            model.IsPublished = true;
            await base.InitializeInsertModel(model);
        }

        protected override async Task InitializeReplaceModel(Intership model)
        {
            model.LastModified = DateTime.Now;
            await base.InitializeReplaceModel(model);
        }

        [HttpPost("import")]
        public async Task<IActionResult> Import(IFormFile file)
        {
            var stream = file.OpenReadStream();
            var reader = new IntershipReader(stream);
            var models = reader.Parse();
            var list = new List<IntershipBuffer>();
            var data = await _service.GetDataForIntershipAction();
            if (!models.Any())
            {
                return BadRequest("File trống");
            }
            foreach (var model in models)
            {
                var row = model.Row;
                var inter = model.Model;
                var buffer = new IntershipBuffer { Intership = new Intership(), Errors = new List<string>() };
                if (string.IsNullOrEmpty(inter.Code))
                {
                    buffer.Errors.Add("Mã trống.");
                    buffer.Index = row;
                }
                else
                {
                    buffer.Intership.DefCode = inter.Code;
                }

                if (string.IsNullOrEmpty(inter.StudentCode))
                {
                    buffer.Errors.Add("Mã sinh viên trống");
                    buffer.Index = row;
                }
                else
                {
                    var employee = data?.Students?.FirstOrDefault(x => x.DefCode == inter.StudentCode);
                    if (employee == null)
                    {
                        buffer.Errors.Add("Không tìm thấy sinh viên trên hệ thống.");
                        buffer.Index = row;
                    }
                    else
                    {
                        buffer.Intership.StudentCode = employee.Id.ToObjectId();
                        buffer.Intership.CenterCode = employee.CenterCode;
                        buffer.Intership.Course = employee.Course;
                        buffer.Intership.MajorCode = employee.MajorCode;
                        // buffer.Intership.Class = employee.Class;
                        buffer.Intership.ClassroomCode = employee.ClassroomCode;
                    }
                }
                buffer.Intership.CreatedDate = DateTime.Now;
                buffer.Intership.LastModified = DateTime.Now;
                buffer.Intership.IsPublished = true;
                if (string.IsNullOrEmpty(inter.FacutlyCode.ToString()))
                {
                    buffer.Errors.Add("Mã khoa trống");
                    buffer.Index = row;
                }
                else
                {
                    var facutly = data.Facutlies.ToList()?.FirstOrDefault(x => x.DefCode == inter.FacutlyCode);
                    if (facutly == null)
                    {
                        buffer.Errors.Add("Không tìm thấy khoa trên hệ thống");
                        buffer.Index = row;
                    }
                    else
                    {
                        buffer.Intership.FacutlyCode = facutly.Id.ToObjectId();
                    }
                }
                if (string.IsNullOrEmpty(inter.Start))
                {
                    buffer.Errors.Add("Ngày bắt đầu trống");
                    buffer.Index = row;
                }
                else
                {
                    buffer.Intership.Start = inter.Start;
                }

                if (string.IsNullOrEmpty(inter.End))
                {
                    buffer.Errors.Add("Ngày kết thúc trống");
                    buffer.Index = row;
                }
                else
                {
                    buffer.Intership.End = inter.End;
                }
                if (!string.IsNullOrEmpty(inter.End) && !string.IsNullOrEmpty(inter.Start))
                {
                    if (TenjinConverts.GetDateTimeExact(inter.Start, "yyyy-MM-dd") > TenjinConverts.GetDateTimeExact(inter.End, "yyyy-MM-dd"))
                    {
                        buffer.Errors.Add("Ngày bắt đầu lớn hơn ngày kết thúc");
                        buffer.Index = row;
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
            var listToInsert = list.Select(x => x.Intership).ToList();
            await _service.Import(listToInsert);
            return Ok();
        }
    }

    public class IntershipBuffer
    {
        public List<string> Errors { get; set; }

        public int Index { get; set; }

        public Intership Intership { get; set; }
    }

}
