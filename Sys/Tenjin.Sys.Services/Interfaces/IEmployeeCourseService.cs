using System.Threading.Tasks;
using Tenjin.Services.Interfaces;
using Tenjin.Sys.Models.Cores;
using Tenjin.Sys.Models.Entities;
using Tenjin.Sys.Models.Views;

namespace Tenjin.Sys.Services.Interfaces
{
    public interface IEmployeeCourseService : IBaseService<EmployeeCourse, EmployeeCourseView>
    {
        Task<bool> Validate(EmployeeCourse entity);

        Task<EmployeeCourseData> GetDataForEmployeeCourseAction();
    }
}
