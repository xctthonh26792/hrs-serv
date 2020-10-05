using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Tenjin.Helpers;
using Tenjin.Models.Attributes;
using Tenjin.Models.Entities;
using Tenjin.Models.Enums;

namespace Tenjin.Sys.Models.Entities
{
    [BsonIgnoreExtraElements]
    public class Employee : BaseEntity
    {
        [BsonProperty(BsonDirection.DESC)]
        public string FirstName { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public string DateOfBirth { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public string Email { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public string Phone { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public ObjectId LevelCode { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public ObjectId MajorCode { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public ObjectId FacutlyCode { get; set; }

        public override string ValueToSearch => $"{Name?.ToSeoUrl()} {FirstName?.ToSeoUrl()} {DefCode?.ToSeoUrl()} {Phone?.ToSeoUrl()} {Email?.ToSeoUrl()}";
    }
}
