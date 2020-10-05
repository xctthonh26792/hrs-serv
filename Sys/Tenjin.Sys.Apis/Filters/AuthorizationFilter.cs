using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tenjin.Helpers;
using Tenjin.Sys.Helpers;

namespace Tenjin.Sys.Apis.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizationFilter : ActionFilterAttribute
    {
        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var token = GetBearerToken(context);
            AuthenticationTicket ticket = OAuthBearerUtils.Instance.Decode(token);
            if ((ticket?.Principal != null) && (ticket?.Properties?.ExpiresUtc >= DateTime.UtcNow))
            {
                context.HttpContext.User = ticket.Principal;
            }
            return base.OnActionExecutionAsync(context, next);
        }

        private static string GetBearerToken(ActionContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(TenjinConstants.AUTHORIZATION_HEADER_NAME, out var values))
            {
                return string.Empty;
            }
            var header = values.FirstOrDefault();
            return TenjinUtils.IsStringNotEmpty(header) && header.StartsWith(TenjinConstants.BEARER_AUTHORIZATION_NAME, StringComparison.OrdinalIgnoreCase)
                ? header
                : string.Empty;
        }
    }
}
