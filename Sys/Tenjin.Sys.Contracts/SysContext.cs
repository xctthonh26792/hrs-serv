using Tenjin.Contracts;
using Tenjin.Contracts.Interfaces;
using Tenjin.Sys.Contracts.Interfaces;
using Tenjin.Sys.Models.Entities;

namespace Tenjin.Sys.Contracts
{
    public class SysContext : BaseContext, ISysContext
    {
        public SysContext(ISysDbBuilder builder) : base(builder)
        {
        }

        public IRepository<User> UserRepository => ResolveRepository<User>();

        public IRepository<Employee> EmployeeRepository => ResolveRepository<Employee>();

        public IRepository<Log> LogRepository => ResolveRepository<Log>();

        public IRepository<CodeGenerate> CodeGenerateRepository => ResolveRepository<CodeGenerate>();

        public IRepository<Facutly> FacutlyRepository => ResolveRepository<Facutly>();

        public IRepository<Center> CenterRepository => ResolveRepository<Center>();

        public IRepository<Major> MajorRepository => ResolveRepository<Major>();

        public IRepository<Level> LevelRepository => ResolveRepository<Level>();

        public IRepository<Course> CourseRepository => ResolveRepository<Course>();

        public IRepository<Student> StudentRepository => ResolveRepository<Student>();

        public IRepository<Intership> IntershipRepository => ResolveRepository<Intership>();

        public IRepository<EmployeeCourse> EmployeeCourseRepository => ResolveRepository<EmployeeCourse>();

        public IRepository<Classroom> ClassroomRepository => ResolveRepository<Classroom>();

        public override IRepository<T> ResolveRepository<T>()
        {
            return new SysRepository<T>(GetDatabase());
        }
    }
}
