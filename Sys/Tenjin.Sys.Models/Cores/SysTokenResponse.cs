using Newtonsoft.Json;
using Tenjin.Sys.Models.Interfaces;

namespace Tenjin.Sys.Models.Cores
{
    public class SysTokenResponse : ITokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("default")]
        public bool IsDefault { get; set; }

        [JsonProperty("permission")]
        public int Permission { get; set; }
    }
}
