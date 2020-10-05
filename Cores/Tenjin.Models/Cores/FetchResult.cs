using Newtonsoft.Json;
using System.Collections.Generic;

namespace Tenjin.Models.Cores
{
    public class FetchResult<T>
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("models")]
        public IEnumerable<T> Models { get; set; }
    }
}
