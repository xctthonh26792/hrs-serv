using MongoDB.Bson.Serialization.Attributes;
using Tenjin.Models.Attributes;
using Tenjin.Models.Entities;
using Tenjin.Models.Enums;

namespace Tenjin.Sys.Models.Entities
{
    [BsonIgnoreExtraElements]
    public class Course : BaseEntity
    {
        [BsonProperty(BsonDirection.DESC)]
        public string Start { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public string End { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public string TimeStart { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public string TimeEnd { get; set; }
    }
}
