using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Tenjin.Helpers;
using Tenjin.Models;
using Tenjin.Sys.Contracts;
using Tenjin.Sys.Contracts.Interfaces;
using Tenjin.Sys.Models.Entities;
using Tenjin.Sys.Services;
using Tenjin.Sys.Services.Interfaces;

namespace Tenjin.Sys.Tools
{
    class Program
    {
        private const string DEFAULT_USER_EMAIL = "xdev.hieudm@gmail.com";

        static async Task Main()
        {
            var watch = new Stopwatch();
            watch.Start();
            var provider = GetProvider();
            await InitializeEmployees(provider);
            await InitializeUsers(provider);
            watch.Stop();
            Console.WriteLine("Time Elapsed {0} ms", watch.Elapsed.TotalMilliseconds);
            Console.WriteLine("COMPLETED.");
            Console.ReadLine();
        }

        static async Task InitializeEmployees(IServiceProvider provider)
        {
            var service = provider.GetRequiredService<IEmployeeService>();
            await service.CreateIndexes();
            var model = new Employee
            {
                DateOfBirth = "1991-10-19",
                Email = DEFAULT_USER_EMAIL,
                FirstName = "hieu",
                IsPublished = true,
                Name = "Do Minh Hieu",
                Phone = "0949328881"
            };
            await service.Add(model);
        }

        
        static async Task InitializeUsers(IServiceProvider provider)
        {
            var service = provider.GetRequiredService<IUserService>();
            await service.CreateIndexes();
            var model = new User
            {
                Code = await GetEmployee(DEFAULT_USER_EMAIL),
                Name = "Do Minh Hieu",
                Username = "hieudm",
                Password = "123123",
                Permission = int.MaxValue,
                IsPublished = true,
                ExtraProps = new Dictionary<string, string>()
            };
            await service.Add(model);

            async Task<ObjectId> GetEmployee(string email)
            {
                var service = provider.GetRequiredService<IEmployeeService>();
                var model = await service.GetSingleByExpression(x => x.Email == email);
                return model == null ? ObjectId.Empty : model.Id.ToObjectId();
            }
        }

        static IServiceProvider GetProvider()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddScoped<ISysDbBuilder, SysDbBuilder>();
            services.AddScoped<ISysContext, SysContext>();
            services.AddScoped<ITokenService, SysTokenService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.ApplySnakeBson();
            services.ApplySnakeJson();
            return services.BuildServiceProvider();
        }
    }
}
