using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Tenjin.Apis.Extensions
{
    public static class TenjinExtensions
    {
        public static string TryGetSingle(this IQueryCollection collection, string name, string def)
        {
            return collection.TryGetValue(name, out var value) ? value.ToString() : def;
        }

        public static IEnumerable<T> Concat<T>(this T value, IEnumerable<T> elements)
        {
            yield return value;
            foreach (var element in elements)
            {
                yield return element;
            }
        }

        private static BsonDocument RenderToBsonDocument<T>(this FilterDefinition<T> filter)
        {
            var registry = BsonSerializer.SerializerRegistry;
            var serializer = registry.GetSerializer<T>();
            return filter.Render(serializer, registry);
        }

        public static string ToJsonString<T>(this FilterDefinition<T> filter)
        {
            if (filter == null)
            {
                return string.Empty;
            }
            return filter.RenderToBsonDocument().ToJson();
        }

        private static BsonDocument RenderToBsonDocument<T>(this SortDefinition<T> filter)
        {
            var registry = BsonSerializer.SerializerRegistry;
            var serializer = registry.GetSerializer<T>();
            return filter.Render(serializer, registry);
        }

        public static string ToJsonString<T>(this SortDefinition<T> filter)
        {
            if (filter == null)
            {
                return string.Empty;
            }
            return filter.RenderToBsonDocument().ToJson();
        }

        public static T TryParseJson<T>(this string value)
        {
            try
            {
                if (IsAssignable<JArray>(typeof(T)))
                {
                    return (T)(object)JArray.Parse(value);
                }
                if (IsAssignable<JToken>(typeof(T)))
                {
                    return (T)(object)JToken.Parse(value);
                }
                if (IsAssignable<JValue>(typeof(T)))
                {
                    return (T)(object)JValue.Parse(value);
                }
                if (IsAssignable<JObject>(typeof(T)))
                {
                    return (T)(object)JObject.Parse(value);
                }
                return default;
            }
            catch
            {
                return default;
            }

            static bool IsAssignable<TC>(Type value)
            {
                return value.IsAssignableFrom(typeof(TC));
            }
        }
    }
}
