using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Tenjin.Helpers;
using Tenjin.Models.Attributes;
using Tenjin.Models.Entities;
using Tenjin.Models.Enums;

namespace Tenjin.Sys.Models.Entities
{
    [BsonIgnoreExtraElements]
    public class Student : BaseEntity
    {
        [BsonProperty(BsonDirection.DESC)]
        public string DateOfBirth { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public string Phone { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public string Email { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public string Course { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public ObjectId MajorCode { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public string Class { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public ObjectId ClassroomCode { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public ObjectId CenterCode { get; set; }

        public override string ValueToSearch => $"{DefCode?.ToSeoUrl()} {Name?.ToSeoUrl()} {Course?.ToSeoUrl()} {Class?.ToSeoUrl()}";
    }
}
