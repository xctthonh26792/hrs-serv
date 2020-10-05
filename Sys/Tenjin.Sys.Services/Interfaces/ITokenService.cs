using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Tenjin.Sys.Models.Entities;
using Tenjin.Sys.Models.Interfaces;

namespace Tenjin.Sys.Services.Interfaces
{
    public interface ITokenService
    {
        Task<Dictionary<string, string>> GetExtraProfile(User user);
        Task<string> GetAccessToken(User user);
        Task<ITokenResponse> GetTokenResponse(User user);
        Task<bool> IsClientCorrect(ClaimsPrincipal principal);
    }
}
