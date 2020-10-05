using System.Collections.Generic;
using System.Threading.Tasks;
using Tenjin.Services.Interfaces;
using Tenjin.Sys.Models.Cores;
using Tenjin.Sys.Models.Entities;
using Tenjin.Sys.Models.Views;

namespace Tenjin.Sys.Services.Interfaces
{
    public interface IStudentService : IBaseService<Student, StudentView>
    {
        Task Import(IEnumerable<Student> entities);

        Task<StudentData> GetStudentDataForAction();
    }
}
