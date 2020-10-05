using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tenjin.Helpers;
using Tenjin.Models;
using Tenjin.Services;
using Tenjin.Sys.Contracts.Interfaces;
using Tenjin.Sys.Models.Entities;
using Tenjin.Sys.Services.Interfaces;

namespace Tenjin.Sys.Services
{
    public class UserService : BaseService<User>, IUserService
    {
        private readonly ISysContext _context;

        public UserService(ISysContext context) : base(context.UserRepository)
        {
            _context = context;
        }

        public override async Task CreateIndexes()
        {
            await base.CreateIndexes();
        }

        public override async Task Add(User entity)
        {
            if (entity == null) return;
            await PasswordValidation(entity.Password?.Trim());
            await UsernameValidation(entity.Username?.Trim());
            entity.Name = entity.Name?.NormalizeString();
            entity.Username = entity.Username?.NormalizeString()?.ToLower(); ;
            entity.Password = entity.Password?.NormalizeString()?.RijndaelHash();
            await base.Add(entity);
        }

        public override async Task Replace(User entity)
        {
            await Task.Yield();
        }

        public async Task<User> GetByUsername(string username)
        {
            if (TenjinUtils.IsStringEmpty(username))
            {
                throw new Exception($"Username is required.");
            }
            username = username.NormalizeString().ToLower();
            return await GetSingleByExpression(x => username.Equals(x.Username));
        }

        public async Task<bool> IsExisted(string username)
        {
            if (TenjinUtils.IsStringEmpty(username))
            {
                throw new Exception($"Username is required.");
            }
            username = username.NormalizeString().ToLower();
            return await Count(x => username.Equals(x.Username)) > 0;
        }

        public async Task<User> Login(string username, string password)
        {
            if (TenjinUtils.IsStringEmpty(password))
            {
                throw new Exception($"Password is required.");
            }
            username = username.NormalizeString().ToLower();
            var user = await GetByUsername(username);
            return TenjinUtils.IsStringEmpty(user?.Password)
                ? default
                : password.Equals(user.Password) || password.VerifyRijndaelHash(user.Password) ? user : default;
        }

        private async Task PasswordValidation(string password)
        {
            if (TenjinUtils.IsStringEmpty(password))
            {
                throw new Exception("Password is required.");
            }
            if (password.Trim().Length < 5)
            {
                throw new Exception("Password is too short.");
            }
            if (password.Trim().Length > 32)
            {
                throw new Exception("Password is too long.");
            }
            await Task.Yield();
        }

        private async Task UsernameValidation(string username)
        {
            if (TenjinUtils.IsStringEmpty(username))
            {
                throw new Exception("Username is required.");
            }
            if (await IsExisted(username))
            {
                throw new Exception("Username is already used.");
            }
        }

        public async Task ChangeProfile(string code, string name, Dictionary<string, string> props)
        {
            var filter = GetFilterExpression<User>(code);
            var updater = Builders<User>.Update.Set(x => x.Name, name).Set(x => x.ExtraProps, props);
            await GetRepository().UpdateOne(filter, updater);
        }

        public async Task<bool> IsLinked(string code)
        {
            if (TenjinUtils.IsStringEmpty(code))
            {
                throw new Exception($"Code is required.");
            }
            return await Count(x => x.Code == code.ToObjectId()) > 0;
        }

        public async Task ChangePassword(string code, string password)
        {
            var filter = GetFilterExpression<User>(code);
            var updater = Builders<User>.Update.Set(x => x.Password, password.RijndaelHash());
            await GetRepository().UpdateOne(filter, updater);
        }
    }
}
