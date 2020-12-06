using MongoDB.Bson.Serialization.Attributes;
using Tenjin.Sys.Models.Entities;

namespace Tenjin.Sys.Models.Views
{
    [BsonIgnoreExtraElements]
    public class IntershipView : Intership
    {
        public Student Student { get; set; }

        public Facutly Facutly { get; set; }

        public Center Center { get; set; }

        public Major Major { get; set; }

        public Classroom Classroom { get; set; }
    }
}
