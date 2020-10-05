using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Tenjin.Models.Attributes;
using Tenjin.Models.Entities;
using Tenjin.Models.Enums;

namespace Tenjin.Sys.Models.Entities
{
    [BsonIgnoreExtraElements]
    public class EmployeeCourse : BaseEntity
    {
        [BsonProperty(BsonDirection.DESC)]
        public ObjectId EmployeeCode { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public ObjectId CourseCode { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public string Start { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public string End { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public string TimeStart { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public string TimeEnd { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public ObjectId FacutlyCode { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public ObjectId MajorCode { get; set; }

        [BsonElement]
        public string CourseStart => $"{Start} {TimeStart}";

        [BsonElement]
        public string CourseEnd => $"{End} {TimeEnd}";
    }
}
