using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Tenjin.Sys.Services;

namespace Tenjin.Sys.Apis.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SysAuthorizationFilter : ActionFilterAttribute
    {
        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                if (HasAllowAnonymous(context))
                {
                    return base.OnActionExecutionAsync(context, next);
                }
                if (Order > 0 && context.Filters.Count(x => x.GetType() == typeof(SysAuthorizationFilter)) > 1)
                {
                    return base.OnActionExecutionAsync(context, next);
                }
                if (!context.HttpContext.User.Identity.IsAuthenticated)
                {
                    return ThrowUnauthorized(context);
                }
                if (context.HttpContext.User.FindFirstValue("Client") != SysTokenService.CLIENT)
                {
                    return ThrowUnauthorized(context);
                }
            }
            catch
            {
                return ThrowUnauthorized(context);
            }
            return base.OnActionExecutionAsync(context, next);
        }

        private static bool HasAllowAnonymous(ActionExecutingContext context)
        {
            if (context.Filters.Count(x => x.GetType() == typeof(AllowAnonymousFilter)) > 0)
            {
                return true;
            }
            return HasControllerAllowAnonymous(context) || HasActionAllowAnonymous(context);
        }

        private static bool HasControllerAllowAnonymous(ActionExecutingContext context)
        {
            var attributes = context.Controller.GetType().GetCustomAttributes(true);
            if (attributes.Count(x => x.GetType() == typeof(AllowAnonymousAttribute)) > 0)
            {
                return true;
            }
            return false;
        }

        private static bool HasActionAllowAnonymous(ActionExecutingContext context)
        {
            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (descriptor != null)
            {
                var attributes = descriptor.MethodInfo.GetCustomAttributes(inherit: true);
                if (attributes.Count(x => x.GetType() == typeof(AllowAnonymousAttribute)) > 0)
                {
                    return true;
                }
            }
            return false;
        }

        private static async Task ThrowUnauthorized(ActionExecutingContext context)
        {
            context.Result = new StatusCodeResult(401);
            await Task.Yield();
        }
    }
}
