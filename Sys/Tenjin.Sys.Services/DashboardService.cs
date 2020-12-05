using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using Tenjin.Helpers;
using Tenjin.Sys.Contracts.Interfaces;
using Tenjin.Sys.Models.Cores;
using Tenjin.Sys.Models.Entities;
using Tenjin.Sys.Services.Interfaces;

namespace Tenjin.Sys.Services
{
    public class DashboardService : IDashboardService
    {
        private ISysContext _context;
        public DashboardService(ISysContext context)
        {
            _context = context;
        }

        public async Task<Dashboard> GetDashboard()
        {
            var from = DateTimeExtensions.GetFirstDayOfMonth(DateTime.Now).ToString("yyyy-MM-dd");
            var to = DateTimeExtensions.GetLastDayOfMonth(DateTime.Now).ToString("yyyy-MM-dd");
            var start_year = DateTimeExtensions.GetFirstDayOfYear(DateTime.Now).ToString("yyyy-MM-dd");
            var end_year = DateTimeExtensions.GetLastDayOfYear(DateTime.Now).ToString("yyyy-MM-dd");
            return new Dashboard
            {
                TotalIntership = await FetchIntership(from, to),
                TotalEmployee = await CountEmployee(),
                TotalStudent = await CountStudent(),
                TotalCourse = await FetchCourse(from, to),
                TotalEmployeeInterships = await FetchIntershipInYear(start_year, end_year)
            };

            async Task<long> CountEmployee()
            {
                return await _context.EmployeeRepository.Count(x => x.IsPublished);
            }

            async Task<long> CountStudent()
            {
                return await _context.StudentRepository.Count(x => x.IsPublished);
            }

            async Task<long> FetchCourse(string from, string to)
            {
                var expression =
                    Builders<EmployeeCourse>.Filter.And(
                        Builders<EmployeeCourse>.Filter.Eq(x => x.IsPublished, true),
                        Builders<EmployeeCourse>.Filter.Or(
                            Builders<EmployeeCourse>.Filter.And(
                                Builders<EmployeeCourse>.Filter.Lte(x => x.Start, from),
                                Builders<EmployeeCourse>.Filter.Gte(x => x.End, to)
                            ),
                            Builders<EmployeeCourse>.Filter.And(
                                Builders<EmployeeCourse>.Filter.Lte(x => x.Start, from),
                                Builders<EmployeeCourse>.Filter.Gte(x => x.End, from)
                            ),
                            Builders<EmployeeCourse>.Filter.And(
                                Builders<EmployeeCourse>.Filter.Gte(x => x.Start, from),
                                Builders<EmployeeCourse>.Filter.Lte(x => x.End, to)
                            ),
                            Builders<EmployeeCourse>.Filter.And(
                                Builders<EmployeeCourse>.Filter.Lte(x => x.Start, to),
                                Builders<EmployeeCourse>.Filter.Gte(x => x.End, to)
                            )
                    )
                );
                return await _context.EmployeeCourseRepository.Count(expression);
            }

            async Task<long> FetchIntershipInYear(string from, string to)
            {
                var expression =
                    Builders<Intership>.Filter.And(
                        Builders<Intership>.Filter.Eq(x => x.IsPublished, true),
                        Builders<Intership>.Filter.Or(
                            Builders<Intership>.Filter.And(
                                Builders<Intership>.Filter.Lte(x => x.Start, from),
                                Builders<Intership>.Filter.Gte(x => x.End, to)
                            ),
                            Builders<Intership>.Filter.And(
                                Builders<Intership>.Filter.Gte(x => x.Start, from),
                                Builders<Intership>.Filter.Lte(x => x.End, to)
                            ),
                            Builders<Intership>.Filter.And(
                                Builders<Intership>.Filter.Lte(x => x.Start, from),
                                Builders<Intership>.Filter.Gte(x => x.End, from)
                            ),
                            Builders<Intership>.Filter.And(
                                Builders<Intership>.Filter.Lte(x => x.Start, to),
                                Builders<Intership>.Filter.Gte(x => x.End, to)
                            )
                    )
                );
                return await _context.IntershipRepository.Count(expression);
            }

            async Task<long> FetchIntership(string from, string to)
            {
                var expression =
                    Builders<Intership>.Filter.And(
                        Builders<Intership>.Filter.Eq(x => x.IsPublished, true),
                        Builders<Intership>.Filter.Or(
                            Builders<Intership>.Filter.And(
                                Builders<Intership>.Filter.Lte(x => x.Start, from),
                                Builders<Intership>.Filter.Gte(x => x.End, to)
                            ),
                            Builders<Intership>.Filter.And(
                                Builders<Intership>.Filter.Gte(x => x.Start, from),
                                Builders<Intership>.Filter.Lte(x => x.End, to)
                            ),
                            Builders<Intership>.Filter.And(
                                Builders<Intership>.Filter.Lte(x => x.Start, from),
                                Builders<Intership>.Filter.Gte(x => x.End, from)
                            ),
                            Builders<Intership>.Filter.And(
                                Builders<Intership>.Filter.Lte(x => x.Start, to),
                                Builders<Intership>.Filter.Gte(x => x.End, to)
                            )
                    )
                );
                return await _context.IntershipRepository.Count(expression);
            }
            
        }
    }
}
