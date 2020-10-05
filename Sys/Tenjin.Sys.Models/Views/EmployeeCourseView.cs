using MongoDB.Bson.Serialization.Attributes;
using Tenjin.Sys.Models.Entities;

namespace Tenjin.Sys.Models.Views
{
    [BsonIgnoreExtraElements]
    public class EmployeeCourseView : EmployeeCourse
    {
        public Employee Employee { get; set; }

        public Course Course { get; set; }

        public Facutly Facutly { get; set; }

        public Major Major { get; set; }
    }
}
