using Tenjin.Contracts.Interfaces;
using Tenjin.Sys.Models.Entities;

namespace Tenjin.Sys.Contracts.Interfaces
{
    public interface ISysContext : IContext
    {
        IRepository<User> UserRepository { get; }
        IRepository<Employee> EmployeeRepository { get; }
        IRepository<CodeGenerate> CodeGenerateRepository { get; }
        IRepository<Log> LogRepository { get; }

        IRepository<Facutly> FacutlyRepository { get; }
        IRepository<Center> CenterRepository { get; }
        IRepository<Major> MajorRepository { get; }
        IRepository<Level> LevelRepository { get; }
        IRepository<Course> CourseRepository { get; }
        IRepository<Student> StudentRepository { get; }
        IRepository<Intership> IntershipRepository { get; }
        IRepository<EmployeeCourse> EmployeeCourseRepository { get; }
        IRepository<Classroom> ClassroomRepository { get; }
    }
}
