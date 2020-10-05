using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Tenjin.Sys.Apis.Converters
{
    public class ObjectIdJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type @type) => typeof(ObjectId).IsAssignableFrom(@type);
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            return ObjectId.TryParse(token.ToObject<string>(), out var @object) ? @object : ObjectId.Empty;
        }
    }
}
