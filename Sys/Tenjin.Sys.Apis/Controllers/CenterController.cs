using Microsoft.AspNetCore.Mvc;
using Tenjin.Apis.Controllers;
using Tenjin.Sys.Models.Entities;
using Tenjin.Sys.Services.Interfaces;

namespace Tenjin.Sys.Apis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CenterController : BaseController<Center>
    {
        private readonly ICenterService _service;
        public CenterController(ICenterService service) : base(service)
        {
            _service = service;
        }
    }
}
