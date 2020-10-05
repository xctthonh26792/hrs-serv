using System.Collections.Generic;
using System.Threading.Tasks;
using Tenjin.Services.Interfaces;
using Tenjin.Sys.Models.Entities;

namespace Tenjin.Sys.Services.Interfaces
{
    public interface IUserService : IBaseService<User>
    {
        Task<User> Login(string username, string password);
        Task<User> GetByUsername(string username);
        Task<bool> IsExisted(string username);
        Task<bool> IsLinked(string code);
        Task ChangeProfile(string code, string name, Dictionary<string, string> props);
        Task ChangePassword(string code, string password);
    }
}
