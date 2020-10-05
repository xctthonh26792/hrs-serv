using Microsoft.AspNetCore.Mvc;
using Tenjin.Apis.Controllers;
using Tenjin.Sys.Models.Entities;
using Tenjin.Sys.Services.Interfaces;

namespace Tenjin.Sys.Apis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LevelController : BaseController<Level>
    {
        public LevelController(ILevelService service) : base(service)
        {

        }
    }
}
