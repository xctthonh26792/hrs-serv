using System.Collections.Generic;
using Tenjin.Sys.Models.Entities;

namespace Tenjin.Sys.Models.Cores
{
    public class EmployeeCourseData
    {
        public IEnumerable<Employee> Employees { get; set; }

        public IEnumerable<Course> Courses { get; set; }
    }
}
