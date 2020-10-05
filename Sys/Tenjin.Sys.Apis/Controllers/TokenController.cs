using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tenjin.Apis.Controllers;
using Tenjin.Helpers;
using Tenjin.Sys.Apis.Models;
using Tenjin.Sys.Helpers;
using Tenjin.Sys.Models.Entities;
using Tenjin.Sys.Services.Interfaces;

namespace Tenjin.Sys.Apis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokenController : TenjinController
    {
        private readonly IUserService _service;
        private readonly ITokenService _token;
        private readonly ILogService _log;
        public TokenController(IUserService service, ITokenService token, ILogService log)
        {
            _service = service;
            _token = token;
            _log = log;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post([FromBody] LoginModel model)
        {
            if (TenjinUtils.IsStringEmpty(model?.Username) || TenjinUtils.IsStringEmpty(model?.Password))
            {
                await Logging("AUTH-TOKEN", model?.Username, "EMPTY");
                return BadRequest();
            }
            var user = await _service.Login(model.Username, model.Password);
            if (user == null)
            {
                await Logging("AUTH-TOKEN", model?.Username, "NOT-FOUND");
                return BadRequest();
            }
            var response = await _token.GetTokenResponse(user);
            await Logging("AUTH-TOKEN", model?.Username, "COMPLETED");
            return Ok(response);
        }

        [HttpPost("renew")]
        public async Task<IActionResult> Renew()
        {
            if (!await _token.IsClientCorrect(User))
            {
                await Logging("AUTH-RENEW", AUTH_USERNAME, "CLIENT-INCORRECT");
                return Unauthorized();
            }
            var token = GetHeader(TenjinConstants.AUTHORIZATION_HEADER_NAME);
            if (TenjinUtils.IsStringEmpty(token) || TenjinUtils.IsStringEmpty(AUTH_CODE))
            {
                await Logging("AUTH-RENEW", AUTH_USERNAME, "TOKEN-EMPTY");
                return Unauthorized();
            }
            var ticket = OAuthBearerUtils.Instance.Decode(token);
            if (ticket?.Principal == null || ticket?.Properties?.ExpiresUtc < DateTime.UtcNow)
            {
                await Logging("AUTH-RENEW", AUTH_USERNAME, "EXPIRED");
                return Unauthorized();
            }
            var user = await _service.GetByCode(AUTH_CODE);
            if (user == null)
            {
                await Logging("AUTH-RENEW", AUTH_USERNAME, "NOT-FOUND");
                return BadRequest();
            }
            var response = await _token.GetTokenResponse(user);
            await Logging("AUTH-RENEW", AUTH_USERNAME, "COMPLETED");
            return Ok(response);
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var user = await _service.GetByCode(AUTH_CODE);
            if (user == null)
            {
                return BadRequest();
            }
            return Ok(new ProfileModel
            {
                Name = user.Name,
                ExtraProps = user.ExtraProps ?? new Dictionary<string, string>()
            });
        }

        [HttpPost("profile")]
        public async Task<IActionResult> ChangeProfile([FromBody] ProfileModel model)
        {
            await _service.ChangeProfile(AUTH_CODE, model.Name, model.ExtraProps ?? new Dictionary<string, string>());
            return Ok();
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            if (TenjinUtils.IsStringEmpty(AUTH_CODE) ||
                TenjinUtils.IsStringEmpty(model?.OldPassword) ||
                TenjinUtils.IsStringEmpty(model?.NewPassword) ||
                model.NewPassword != model.ConfirmPassword)
            {
                return BadRequest();
            }
            var user = await _service.GetByCode(AUTH_CODE);
            if (!model.OldPassword.Equals(user.Password) && !model.OldPassword.VerifyRijndaelHash(user.Password))
            {
                return BadRequest();
            }
            await _service.ChangePassword(user.Id, model.NewPassword);
            return Ok();
        }

        public async Task Logging(string routing, string username, string status)
        {
            var model = new Log
            {
                IP = GetAddress(),
                Username = username,
                Path = routing,
                Status = status
            };
            await _log.Add(model);
        }

        private string GetAddress()
        {
            var address = GetHeader("TENJIN-CLIENT-IP");
            if (TenjinUtils.IsStringNotEmpty(address))
            {
                return address;
            }
            address = GetHeader("CF-Connecting-IP");
            if (TenjinUtils.IsStringNotEmpty(address))
            {
                return address;
            }
            address = GetHeader("X-Forwarded-For");
            if (TenjinUtils.IsStringNotEmpty(address))
            {
                return address;
            }
            address = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            if (TenjinUtils.IsStringNotEmpty(address))
            {
                return address;
            }
            return string.Empty;
        }
    }
}
