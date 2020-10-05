using System.Collections.Generic;

namespace Tenjin.Sys.Apis.Models
{
    public class ProfileModel
    {
        public string Name { get; set; }
        public Dictionary<string, string> ExtraProps { get; set; }
    }
}
