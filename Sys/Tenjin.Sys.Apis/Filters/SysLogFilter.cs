using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Tenjin.Helpers;
using Tenjin.Sys.Models.Entities;
using Tenjin.Sys.Services.Interfaces;

namespace Tenjin.Sys.Apis.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SysLogFilter : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var path = GetPath(context);
            if (path.StartsWith("/token"))
            {
                await base.OnActionExecutionAsync(context, next);
                return;
            }
            var service = context.HttpContext.RequestServices.GetService<ILogService>();
            var model = new Log
            {
                IP = GetAddress(context),
                Path = path,
                Username = GetUser(context),
                Status = $"REQUEST-{context.HttpContext.Request.Method.ToUpper()}"
            };
            await service.Add(model);
            await base.OnActionExecutionAsync(context, next);
        }

        private string GetAddress(ActionExecutingContext context)
        {
            var address = GetHeader(context, "TENJIN-CLIENT-IP");
            if (TenjinUtils.IsStringNotEmpty(address))
            {
                return address;
            }
            address = GetHeader(context, "CF-Connecting-IP");
            if (TenjinUtils.IsStringNotEmpty(address))
            {
                return address;
            }
            address = GetHeader(context, "X-Forwarded-For");
            if (TenjinUtils.IsStringNotEmpty(address))
            {
                return address;
            }
            address = context.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            if (TenjinUtils.IsStringNotEmpty(address))
            {
                return address;
            }
            return string.Empty;
        }

        private string GetHeader(ActionExecutingContext context, string name)
        {
            return context.HttpContext.Request.Headers?.FirstOrDefault(x => x.Key.Equals(name, StringComparison.OrdinalIgnoreCase)).Value ?? string.Empty;
        }

        private string GetPath(ActionExecutingContext context)
        {
            return context.HttpContext.Request.Path.ToString();
        }

        private string GetUser(ActionExecutingContext context)
        {
            return context.HttpContext.User?.FindFirstValue(ClaimTypes.Email);
        }
    }
}
