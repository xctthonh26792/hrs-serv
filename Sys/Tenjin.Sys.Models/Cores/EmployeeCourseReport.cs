using MongoDB.Bson.Serialization.Attributes;

namespace Tenjin.Sys.Models.Cores
{
    [BsonIgnoreExtraElements]
    public class EmployeeCourseReport
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string DateOfBirth { get; set; }

        public string Facutly { get; set; }

        public string Major { get; set; }

        public double TotalTime { get; set; }
    }
}
