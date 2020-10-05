using System.Collections.Generic;
using Tenjin.Sys.Models.Entities;

namespace Tenjin.Sys.Models.Cores
{
    public class StudentData
    {
        public IEnumerable<Center> Centers { get; set; }

        public IEnumerable<Major> Majors { get; set; }
    }
}
