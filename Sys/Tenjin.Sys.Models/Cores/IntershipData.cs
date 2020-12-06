using System.Collections.Generic;
using Tenjin.Sys.Models.Entities;

namespace Tenjin.Sys.Models.Cores
{
    public class IntershipData
    {
        public IEnumerable<Student> Students { get; set; }

        public IEnumerable<Facutly> Facutlies { get; set; }

        public IEnumerable<Classroom> Classrooms { get; set; }
    }
}
