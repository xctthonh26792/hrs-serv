using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Tenjin.Models.Attributes;
using Tenjin.Models.Entities;
using Tenjin.Models.Enums;

namespace Tenjin.Sys.Models.Entities
{
    [BsonIgnoreExtraElements]
    public class Intership : BaseEntity
    {
        [BsonProperty(BsonDirection.DESC)]
        public ObjectId StudentCode { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public ObjectId FacutlyCode { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public ObjectId CenterCode { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public string Course { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public ObjectId MajorCode { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public string Class { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public string Start { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public string End { get; set; }
    }
}
