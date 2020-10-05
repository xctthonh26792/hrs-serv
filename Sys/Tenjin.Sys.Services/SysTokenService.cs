using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Tenjin.Helpers;
using Tenjin.Sys.Contracts.Interfaces;
using Tenjin.Sys.Helpers;
using Tenjin.Sys.Models.Cores;
using Tenjin.Sys.Models.Entities;
using Tenjin.Sys.Models.Interfaces;
using Tenjin.Sys.Services.Interfaces;

namespace Tenjin.Sys.Services
{
    public class SysTokenService : ITokenService
    {
        public const string CLIENT = "TENJIN_SYSTEM";
        private readonly ISysContext _context;
        public SysTokenService(ISysContext context)
        {
            _context = context;
        }

        public async Task<string> GetAccessToken(User user)
        {
            var extras = await GetExtraProfile(user);
            return OAuthBearerUtils.Instance.GenerateToken(user.Id, user.Username, TenjinConstants.TOKEN_TIMEOUT, extras);
        }

        public Task<Dictionary<string, string>> GetExtraProfile(User user)
        {
            var mappings = new Dictionary<string, string>
            {
                [TenjinConstants.PERMISSION_HEADER] = user.Permission.ToString(),
                ["Client"] = CLIENT
            };
            return Task.FromResult(mappings);
        }

        public async Task<ITokenResponse> GetTokenResponse(User user)
        {
            var employee = await _context.EmployeeRepository.GetSingleByExpression(x => x.Id == user.Code.ToString());
            if (employee == null || !employee.IsPublished)
            {
                throw new Exception("Employee has been disabled.");
            }
            return new SysTokenResponse
            {
                AccessToken = await GetAccessToken(user),
                Code = employee.Id,
                Name = user.Name ?? employee.Name,
                IsDefault = user.Permission == int.MaxValue,
                Permission = user.Permission
            };
        }

        public Task<bool> IsClientCorrect(ClaimsPrincipal principal)
        {
            var client = principal?.FindFirstValue("Client");
            return Task.FromResult(CLIENT.Equals(client));
        }
    }
}
