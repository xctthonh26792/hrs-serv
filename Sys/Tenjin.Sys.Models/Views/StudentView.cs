using MongoDB.Bson.Serialization.Attributes;
using Tenjin.Sys.Models.Entities;

namespace Tenjin.Sys.Models.Views
{
    [BsonIgnoreExtraElements]
    public class StudentView : Student
    {
        public Center Center { get; set; }

        public Major Major { get; set; }
    }
}
