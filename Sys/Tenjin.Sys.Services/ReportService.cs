using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tenjin.Models;
using Tenjin.Sys.Models.Cores;
using Tenjin.Sys.Models.Entities;
using Tenjin.Sys.Models.Views;
using Tenjin.Sys.Services.Interfaces;

namespace Tenjin.Sys.Services
{
    public class ReportService : IReportService
    {
        private readonly IServiceProvider _provider;
        public ReportService(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task<IEnumerable<EmployeeCourseView>> GetEmployeeCourseByEmployeeAndTime(ReportQuery query)
        {
            var service = _provider.GetRequiredService<IEmployeeCourseService>();
            var expression =
                    Builders<EmployeeCourse>.Filter.And(
                        Builders<EmployeeCourse>.Filter.Eq(x => x.IsPublished, true),
                        Builders<EmployeeCourse>.Filter.Or(
                            Builders<EmployeeCourse>.Filter.And(
                                Builders<EmployeeCourse>.Filter.Lte(x => x.Start, query.Start),
                                Builders<EmployeeCourse>.Filter.Gte(x => x.End, query.Start)
                            ),
                            Builders<EmployeeCourse>.Filter.And(
                                Builders<EmployeeCourse>.Filter.Lte(x => x.Start, query.Start),
                                Builders<EmployeeCourse>.Filter.Gte(x => x.End, query.End)
                            ),
                            Builders<EmployeeCourse>.Filter.And(
                                Builders<EmployeeCourse>.Filter.Gte(x => x.Start, query.Start),
                                Builders<EmployeeCourse>.Filter.Lte(x => x.End, query.End)
                            ),
                            Builders<EmployeeCourse>.Filter.And(
                                Builders<EmployeeCourse>.Filter.Lte(x => x.Start, query.End),
                                Builders<EmployeeCourse>.Filter.Gte(x => x.End, query.End)
                            )
                    )
                );
            if (!query.IsAllEmployee)
            {
                expression = Builders<EmployeeCourse>.Filter.And(expression, Builders<EmployeeCourse>.Filter.In(x => x.EmployeeCode, query.EmployeeCodes));
            }
            return await service.GetByExpression(expression);
        }

        public async Task<IEnumerable<EmployeeCourseReport>> GetTotalTimeByEmployeeAndTime(ReportQuery query)
        {
            var service = _provider.GetRequiredService<IEmployeeCourseService>();
            var expression =
                    Builders<EmployeeCourse>.Filter.And(
                        Builders<EmployeeCourse>.Filter.Eq(x => x.IsPublished, true),
                        Builders<EmployeeCourse>.Filter.Or(
                            Builders<EmployeeCourse>.Filter.And(
                                Builders<EmployeeCourse>.Filter.Lte(x => x.Start, query.Start),
                                Builders<EmployeeCourse>.Filter.Gte(x => x.End, query.Start)
                            ),
                            Builders<EmployeeCourse>.Filter.And(
                                Builders<EmployeeCourse>.Filter.Lte(x => x.Start, query.Start),
                                Builders<EmployeeCourse>.Filter.Gte(x => x.End, query.End)
                            ),
                            Builders<EmployeeCourse>.Filter.And(
                                Builders<EmployeeCourse>.Filter.Gte(x => x.Start, query.Start),
                                Builders<EmployeeCourse>.Filter.Lte(x => x.End, query.End)
                            ),
                            Builders<EmployeeCourse>.Filter.And(
                                Builders<EmployeeCourse>.Filter.Lte(x => x.Start, query.End),
                                Builders<EmployeeCourse>.Filter.Gte(x => x.End, query.End)
                            )
                    )
                );
            if (!query.IsAllFacutly)
            {
                expression = Builders<EmployeeCourse>.Filter.And(expression, Builders<EmployeeCourse>.Filter.In(x => x.FacutlyCode, query.FacutlyCodes));
            }
            var datas =  await service.GetByExpression(expression);
            if (datas == null || !datas.Any()) return null;
            return datas.GroupBy(x => x.Employee.Id).Select(x => new EmployeeCourseReport
            {
                Code = x.FirstOrDefault()?.Employee?.DefCode,
                Name = x.FirstOrDefault()?.Employee?.Name,
                DateOfBirth = x.FirstOrDefault()?.Employee?.DateOfBirth,
                Facutly = x.FirstOrDefault()?.Facutly?.Name,
                Major = x.FirstOrDefault()?.Major?.Name,
                TotalTime = x.Sum(x => x.CourseTime)
            });
        }

        public async Task<IEnumerable<EmployeeCourseView>> GetEmployeeCourseByFacutly(ReportQuery query)
        {
            var service = _provider.GetRequiredService<IEmployeeCourseService>();
            var expression =
                    Builders<EmployeeCourse>.Filter.And(
                        Builders<EmployeeCourse>.Filter.Eq(x => x.IsPublished, true),
                        Builders<EmployeeCourse>.Filter.Or(
                            Builders<EmployeeCourse>.Filter.And(
                                Builders<EmployeeCourse>.Filter.Lte(x => x.Start, query.Start),
                                Builders<EmployeeCourse>.Filter.Gte(x => x.End, query.Start)
                            ),
                            Builders<EmployeeCourse>.Filter.And(
                                Builders<EmployeeCourse>.Filter.Lte(x => x.Start, query.Start),
                                Builders<EmployeeCourse>.Filter.Gte(x => x.End, query.End)
                            ),
                            Builders<EmployeeCourse>.Filter.And(
                                Builders<EmployeeCourse>.Filter.Gte(x => x.Start, query.Start),
                                Builders<EmployeeCourse>.Filter.Lte(x => x.End, query.End)
                            ),
                            Builders<EmployeeCourse>.Filter.And(
                                Builders<EmployeeCourse>.Filter.Lte(x => x.Start, query.End),
                                Builders<EmployeeCourse>.Filter.Gte(x => x.End, query.End)
                            )
                    )
                );
            if (!query.IsAllFacutly)
            {
                expression = Builders<EmployeeCourse>.Filter.And(expression, Builders<EmployeeCourse>.Filter.In(x => x.FacutlyCode, query.FacutlyCodes));
            }
            return await service.GetByExpression(expression);
        }

        public async Task<IEnumerable<EmployeeCourseView>> GetCourseByCourse(ReportQuery query)
        {
            var service = _provider.GetRequiredService<IEmployeeCourseService>();
            var expression =
                    Builders<EmployeeCourse>.Filter.And(
                        Builders<EmployeeCourse>.Filter.Eq(x => x.IsPublished, true),
                        Builders<EmployeeCourse>.Filter.Or(
                            Builders<EmployeeCourse>.Filter.And(
                                Builders<EmployeeCourse>.Filter.Lte(x => x.Start, query.Start),
                                Builders<EmployeeCourse>.Filter.Gte(x => x.End, query.Start)
                            ),
                            Builders<EmployeeCourse>.Filter.And(
                                Builders<EmployeeCourse>.Filter.Lte(x => x.Start, query.Start),
                                Builders<EmployeeCourse>.Filter.Gte(x => x.End, query.End)
                            ),
                            Builders<EmployeeCourse>.Filter.And(
                                Builders<EmployeeCourse>.Filter.Gte(x => x.Start, query.Start),
                                Builders<EmployeeCourse>.Filter.Lte(x => x.End, query.End)
                            ),
                            Builders<EmployeeCourse>.Filter.And(
                                Builders<EmployeeCourse>.Filter.Lte(x => x.Start, query.End),
                                Builders<EmployeeCourse>.Filter.Gte(x => x.End, query.End)
                            )
                    )
                );
            if (!query.IsAllCourse)
            {
                expression = Builders<EmployeeCourse>.Filter.And(expression, Builders<EmployeeCourse>.Filter.In(x => x.CourseCode, query.CourseCodes));
            }
            return  await service.GetByExpression(expression);
        }

        public async Task<IEnumerable<IntershipView>> GetIntershipByStudent(string code)
        {
            var service = _provider.GetRequiredService<IIntershipService>();
            var expression =
                    Builders<Intership>.Filter.And(
                        Builders<Intership>.Filter.Eq(x => x.IsPublished, true),
                        Builders<Intership>.Filter.Eq(x => x.StudentCode, code.ToObjectId()));
            return await service.GetByExpression(expression);
        }

        public async Task<IEnumerable<IntershipView>> GetIntershipByFacutlyAndTime(ReportQuery query)
        {
            var service = _provider.GetRequiredService<IIntershipService>();
            var expression =
                    Builders<Intership>.Filter.And(
                        Builders<Intership>.Filter.Eq(x => x.IsPublished, true),
                        Builders<Intership>.Filter.Or(
                            Builders<Intership>.Filter.And(
                                Builders<Intership>.Filter.Lte(x => x.Start, query.Start),
                                Builders<Intership>.Filter.Gte(x => x.End, query.Start)
                            ),
                            Builders<Intership>.Filter.And(
                                Builders<Intership>.Filter.Lte(x => x.Start, query.Start),
                                Builders<Intership>.Filter.Gte(x => x.End, query.End)
                            ),
                            Builders<Intership>.Filter.And(
                                Builders<Intership>.Filter.Gte(x => x.Start, query.Start),
                                Builders<Intership>.Filter.Lte(x => x.End, query.End)
                            ),
                            Builders<Intership>.Filter.And(
                                Builders<Intership>.Filter.Lte(x => x.Start, query.End),
                                Builders<Intership>.Filter.Gte(x => x.End, query.End)
                            )
                    )
                );
            if (!query.IsAllFacutly)
            {
                expression = Builders<Intership>.Filter.And(expression, Builders<Intership>.Filter.In(x => x.FacutlyCode, query.FacutlyCodes));
            }
            return await service.GetByExpression(expression);

        }

        public async Task<IEnumerable<IntershipView>> GetIntershipByCenterAndTime(ReportQuery query)
        {
            var service = _provider.GetRequiredService<IIntershipService>();
            var expression =
                    Builders<Intership>.Filter.And(
                        Builders<Intership>.Filter.Eq(x => x.IsPublished, true),
                        Builders<Intership>.Filter.Or(
                            Builders<Intership>.Filter.And(
                                Builders<Intership>.Filter.Lte(x => x.Start, query.Start),
                                Builders<Intership>.Filter.Gte(x => x.End, query.Start)
                            ),
                            Builders<Intership>.Filter.And(
                                Builders<Intership>.Filter.Lte(x => x.Start, query.Start),
                                Builders<Intership>.Filter.Gte(x => x.End, query.End)
                            ),
                            Builders<Intership>.Filter.And(
                                Builders<Intership>.Filter.Gte(x => x.Start, query.Start),
                                Builders<Intership>.Filter.Lte(x => x.End, query.End)
                            ),
                            Builders<Intership>.Filter.And(
                                Builders<Intership>.Filter.Lte(x => x.Start, query.End),
                                Builders<Intership>.Filter.Gte(x => x.End, query.End)
                            )
                    )
                );
            if (!query.IsAllCenter)
            {
                expression = Builders<Intership>.Filter.And(expression, Builders<Intership>.Filter.In(x => x.CenterCode, query.CenterCodes));
            }
            return await service.GetByExpression(expression);
        }
    }
}
