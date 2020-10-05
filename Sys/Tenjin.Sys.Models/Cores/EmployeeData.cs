using System.Collections.Generic;
using Tenjin.Sys.Models.Entities;

namespace Tenjin.Sys.Models.Cores
{
    public class EmployeeData
    {
        public IEnumerable<Level> Levels { get; set; }

        public IEnumerable<Facutly> Facutlies { get; set; }

        public IEnumerable<Major> Majors { get; set; }
    }
}
