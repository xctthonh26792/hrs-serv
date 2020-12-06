using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Tenjin.Apis.Controllers;
using Tenjin.Sys.Models.Cores;
using Tenjin.Sys.Services.Interfaces;

namespace Tenjin.Sys.Apis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportController : TenjinController
    {
        private readonly IReportService _service;
        public ReportController(IReportService service)
        {
            _service = service;
        }

        [HttpGet("intershipbystudent/{code}")]
        public async Task<IActionResult> GetIntershipByStudent(string code)
        {
            return Ok(await _service.GetIntershipByStudent(code));
        }

        [HttpPost("coursebyfacutly")]
        public async Task<IActionResult> GetCourseByFacutly([FromBody] ReportQuery query)
        {
            return Ok(await _service.GetEmployeeCourseByFacutly(query));
        }

        [HttpPost("coursebyemployee")]
        public async Task<IActionResult> GetCourseByEmployee([FromBody] ReportQuery query)
        {
            return Ok(await _service.GetEmployeeCourseByEmployeeAndTime(query));
        }

        [HttpPost("coursebycourse")]
        public async Task<IActionResult> GetCourseByCourse([FromBody] ReportQuery query)
        {
            return Ok(await _service.GetCourseByCourse(query));
        }

        [HttpPost("intershipbycenter")]
        public async Task<IActionResult> GetIntershipByCenterAndTime([FromBody] ReportQuery query)
        {
            return Ok(await _service.GetIntershipByCenterAndTime(query));
        }

        [HttpPost("intershipbyfacutly")]
        public async Task<IActionResult> GetIntershipByFacutlyAndTime([FromBody] ReportQuery query)
        {
            return Ok(await _service.GetIntershipByFacutlyAndTime(query));
        }

        [HttpPost("intershipbyclassroom")]
        public async Task<IActionResult> GetIntershipByClassroomAndTime([FromBody] ReportQuery query)
        {
            return Ok(await _service.GetIntershipByClassroomAndTime(query));
        }

        [HttpPost("coursetime")]
        public async Task<IActionResult> GetTotalTimeByEmployeeAndTime([FromBody] ReportQuery query)
        {
            return Ok(await _service.GetTotalTimeByEmployeeAndTime(query));
        }
        
    }
}
