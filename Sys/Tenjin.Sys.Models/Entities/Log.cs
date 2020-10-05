using Tenjin.Models.Entities;

namespace Tenjin.Sys.Models.Entities
{
    public class Log : BaseEntity
    {
        public string IP { get; set; }
        public string Username { get; set; }
        public string Path { get; set; }
        public string Status { get; set; }
    }
}
