using Microsoft.AspNetCore.Mvc;
using Tenjin.Apis.Controllers;
using Tenjin.Sys.Models.Entities;
using Tenjin.Sys.Services.Interfaces;

namespace Tenjin.Sys.Apis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FacutlyController : BaseController<Facutly>
    {
        private readonly IFacutlyService _service;
        public FacutlyController(IFacutlyService service) : base(service)
        {
            _service = service;
        }
    }
}
