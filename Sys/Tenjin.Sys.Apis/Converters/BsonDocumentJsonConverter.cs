using MongoDB.Bson;
using Newtonsoft.Json;
using System;

namespace Tenjin.Sys.Apis.Converters
{
    public class BsonDocumentJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type @type) => @type == typeof(BsonDocument);
        public override bool CanRead => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var settings = new MongoDB.Bson.IO.JsonWriterSettings { OutputMode = MongoDB.Bson.IO.JsonOutputMode.Strict };
            string json = (value as BsonDocument).ToJson(settings);
            writer.WriteRawValue(json);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
