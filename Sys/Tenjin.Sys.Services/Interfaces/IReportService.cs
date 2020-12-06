using System.Collections.Generic;
using System.Threading.Tasks;
using Tenjin.Sys.Models.Cores;
using Tenjin.Sys.Models.Views;

namespace Tenjin.Sys.Services.Interfaces
{
    public interface IReportService
    {
        Task<IEnumerable<IntershipView>> GetIntershipByStudent(string code);

        Task<IEnumerable<EmployeeCourseView>> GetEmployeeCourseByEmployeeAndTime(ReportQuery query);

        Task<IEnumerable<EmployeeCourseReport>> GetTotalTimeByEmployeeAndTime(ReportQuery query);

        Task<IEnumerable<EmployeeCourseView>> GetEmployeeCourseByFacutly(ReportQuery query);

        Task<IEnumerable<EmployeeCourseView>> GetCourseByCourse(ReportQuery query);

        Task<IEnumerable<IntershipView>> GetIntershipByFacutlyAndTime(ReportQuery query);

        Task<IEnumerable<IntershipView>> GetIntershipByClassroomAndTime(ReportQuery query);

        Task<IEnumerable<IntershipView>> GetIntershipByCenterAndTime(ReportQuery query);
    }
}
