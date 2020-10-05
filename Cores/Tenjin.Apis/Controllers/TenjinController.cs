using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;

namespace Tenjin.Apis.Controllers
{
    public abstract class TenjinController : ControllerBase
    {
        protected string AUTH_CODE => User?.FindFirstValue(ClaimTypes.NameIdentifier);
        protected string AUTH_USERNAME => User?.FindFirstValue(ClaimTypes.Email);
        protected string GetPrinciple(string name) => User?.FindFirstValue(name);
        protected string GetHeader(string name) => Request?.Headers?.FirstOrDefault(x => x.Key.Equals(name, StringComparison.OrdinalIgnoreCase)).Value ?? string.Empty;
    }
}
