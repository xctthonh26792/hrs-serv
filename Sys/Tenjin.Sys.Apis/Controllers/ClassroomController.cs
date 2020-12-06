using Microsoft.AspNetCore.Mvc;
using Tenjin.Apis.Controllers;
using Tenjin.Sys.Models.Entities;
using Tenjin.Sys.Services.Interfaces;

namespace Tenjin.Sys.Apis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClassroomController : BaseController<Classroom>
    {
        private readonly IClassroomService _service;
        public ClassroomController(IClassroomService service): base(service)
        {
            _service = service;
        }
    }
}
