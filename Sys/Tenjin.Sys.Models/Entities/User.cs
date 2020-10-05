using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System.Collections.Generic;
using Tenjin.Models.Attributes;
using Tenjin.Models.Entities;
using Tenjin.Models.Enums;

namespace Tenjin.Sys.Models.Entities
{
    [BsonIgnoreExtraElements]
    public class User : BaseEntity
    {
        public User()
        {
            ExtraProps = new Dictionary<string, string>();
        }

        [BsonProperty(BsonDirection.DESC)]
        public ObjectId Code { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public string Username { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        [BsonProperty(BsonDirection.DESC)]
        public int Permission { get; set; }

        public Dictionary<string, string> ExtraProps { get; set; }
    }
}
