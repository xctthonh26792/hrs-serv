using MongoDB.Bson.Serialization.Attributes;
using Tenjin.Sys.Models.Entities;

namespace Tenjin.Sys.Models.Views
{
    [BsonIgnoreExtraElements]
    public class EmployeeView : Employee
    {
        public bool HasUser { get; set; }
        public int Permission { get; set; }
        public string Username { get; set; }
        public Facutly Facutly { get; set; }
        public Level Level { get; set; }
        public Major Major { get; set; }


    }
}
