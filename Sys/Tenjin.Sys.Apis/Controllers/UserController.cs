using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Tenjin.Apis.Controllers;
using Tenjin.Apis.Reflections;
using Tenjin.Helpers;
using Tenjin.Models;
using Tenjin.Sys.Apis.Models;
using Tenjin.Sys.Models.Entities;
using Tenjin.Sys.Services.Interfaces;

namespace Tenjin.Sys.Apis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : BaseController
    {
        private readonly IUserService _service;
        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpGet("{page:int}/{quantity:int}")]
        [HttpPost("{page:int}/{quantity:int}")]
        public virtual async Task<IActionResult> GetPageByExpression(int page, int quantity)
        {
            var context = await HttpContextReader.Create(HttpContext);
            return Ok(await _service.GetPageByExpression(context.As<User>(), page, quantity));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (TenjinUtils.IsStringEmpty(model?.Username) || TenjinUtils.IsStringEmpty(model?.Password) || TenjinUtils.IsStringEmpty(model?.Code))
            {
                return BadRequest();
            }
            if (model.Code.IsObjectIdEmpty())
            {
                return BadRequest();
            }
            var linked = await _service.IsLinked(model.Code);
            var existed = await _service.IsExisted(model.Username);
            if (existed || linked)
            {
                return BadRequest();
            }
            await _service.Add(new User
            {
                Code = model.Code.ToObjectId(),
                Name = model.Name.NormalizeString(),
                Username = model.Username.NormalizeString(),
                Password = model.Password,
                Permission = model.Permission
            });
            return Ok();
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            if (TenjinUtils.IsStringEmpty(model?.Code) ||
                TenjinUtils.IsStringEmpty(model?.NewPassword) ||
                model.NewPassword != model.ConfirmPassword)
            {
                return BadRequest();
            }
            var user = await _service.GetSingleByExpression(x => x.Code == model.Code.ToObjectId());
            if (user == null)
            {
                return BadRequest();
            }
            await _service.ChangePassword(user.Id, model.NewPassword);
            return Ok();
        }
    }
}
