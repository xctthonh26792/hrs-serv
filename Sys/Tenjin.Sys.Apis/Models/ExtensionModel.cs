using Newtonsoft.Json;

namespace Tenjin.Sys.Apis.Models
{
    public class ExtensionModel
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("expired")]
        public string ExpiredDate { get; set; }
    }
}
