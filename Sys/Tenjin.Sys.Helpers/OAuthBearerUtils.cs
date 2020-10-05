using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Tenjin.Helpers;

namespace Tenjin.Sys.Helpers
{
    public class OAuthBearerUtils
    {
        private static OAuthBearerUtils _instance;
        private readonly TicketDataFormat _format;

        private OAuthBearerUtils()
        {
            _format = new TicketDataFormat(new TenjinDataProtector());
        }

        public static OAuthBearerUtils Instance => _instance ?? (_instance = new OAuthBearerUtils());

        public string GenerateToken(string code, string name, int timeout, Dictionary<string, string> extras = null)
        {
            var principal = CreatePrincipal(code, name, extras);
            var props = CreateProperties(timeout);
            return _format.Protect(new AuthenticationTicket(principal, props, TenjinConstants.BEARER_AUTHORIZATION_NAME));
        }

        public AuthenticationTicket Decode(string token)
        {
            try
            {
                if (TenjinUtils.IsStringEmpty(token))
                {
                    return default;
                }
                var value = token?.Replace(TenjinConstants.BEARER_AUTHORIZATION_NAME, string.Empty)?.Trim();
                if (TenjinUtils.IsStringEmpty(value))
                {
                    return default;
                }
                return _format.Unprotect(value);
            }
            catch
            {
                return default;
            }
        }

        private ClaimsPrincipal CreatePrincipal(string code, string name, Dictionary<string, string> extras = null)
        {
            var claims = GetClaims(code, name, extras);
            return new ClaimsPrincipal(new ClaimsIdentity(claims, TenjinConstants.BEARER_AUTHORIZATION_NAME));
        }

        private IEnumerable<Claim> GetClaims(string code, string name, Dictionary<string, string> extras = null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, code),
                new Claim(ClaimTypes.Email, name)
            };
            if (extras != null && extras.Any())
            {
                foreach (var extra in extras)
                {
                    claims.Add(new Claim(extra.Key, extra.Value));
                }
            }
            return claims;
        }

        private AuthenticationProperties CreateProperties(int timeout)
        {
            var current = DateTime.UtcNow;
            return new AuthenticationProperties
            {
                IssuedUtc = current,
                ExpiresUtc = current.Add(TimeSpan.FromDays(timeout))
            };
        }
    }
}
