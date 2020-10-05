using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Tenjin.Apis.Controllers;
using Tenjin.Sys.Services.Interfaces;

namespace Tenjin.Sys.Apis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DashboardController : TenjinController
    {
        private readonly IDashboardService _service;
        public DashboardController(IDashboardService service)
        {
            _service = service;
        }

        [HttpGet("dashboarddata")]
        public async Task<IActionResult> FetchDashboard()
        {
            return Ok(await _service.GetDashboard());
        }
    }
}
