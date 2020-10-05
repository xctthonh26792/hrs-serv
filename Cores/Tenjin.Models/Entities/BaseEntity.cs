using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using Tenjin.Helpers;
using Tenjin.Models.Attributes;
using Tenjin.Models.Enums;
using Tenjin.Models.Interfaces;

namespace Tenjin.Models.Entities
{
    public class BaseEntity : IEntity
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public virtual string DefCode { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public virtual string Name { get; set; }

        [BsonIgnoreIfNull, BsonIgnoreIfDefault]
        public virtual string Description { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public DateTime CreatedDate { get; set; }

        public DateTime LastModified { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public bool IsPublished { get; set; }

        [BsonElement]
        [BsonProperty(BsonDirection.DESC)]
        public virtual string ValueToSearch => $"{DefCode?.ToSeoUrl()} {Name?.ToSeoUrl()}";
    }
}
