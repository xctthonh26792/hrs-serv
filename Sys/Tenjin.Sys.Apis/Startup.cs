using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tenjin.Apis;
using Tenjin.Helpers;
using Tenjin.Sys.Apis.Converters;
using Tenjin.Sys.Apis.Filters;
using Tenjin.Sys.Contracts;
using Tenjin.Sys.Contracts.Interfaces;
using Tenjin.Sys.Services;
using Tenjin.Sys.Services.Interfaces;

namespace Tenjin.Sys.Apis
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor());

            services
                .AddControllers(options =>
                {
                    options.Filters.Add(typeof(AuthorizationFilter), 0);
                    options.Filters.Add(typeof(SysAuthorizationFilter), 1);
                    options.Filters.Add(typeof(SysLogFilter), 2);
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ApplySnakeJson();
                });
            services.Configure<MvcNewtonsoftJsonOptions>(options =>
            {
                options.SerializerSettings.Converters.Add(new BsonDocumentJsonConverter());
                options.SerializerSettings.Converters.Add(new ObjectIdJsonConverter());
            });
            services.AddScoped<ISysDbBuilder, SysDbBuilder>();
            services.AddScoped<ISysContext, SysContext>();
            services.AddScoped<ITokenService, SysTokenService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IFacutlyService, FacutlyService>();
            services.AddScoped<ICenterService, CenterService>();
            services.AddScoped<IMajorService, MajorService>();
            services.AddScoped<ILevelService, LevelService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IIntershipService, IntershipService>();
            services.AddScoped<IEmployeeCourseService, EmployeeCourseService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<ILogService, LogService>();

            TenjinStartupHelper.ConfigureServices(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = (handler) =>
                {
                    handler.Context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                }
            });
            TenjinStartupHelper.Configure(app);
        }
    }
}
