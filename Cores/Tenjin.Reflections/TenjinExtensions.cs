using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using Tenjin.Reflections.Serializations;

namespace Tenjin.Helpers
{
    public static partial class TenjinExtensions
    {
        public static JsonSerializerSettings ApplySnakeJson(this JsonSerializerSettings settings)
        {
            if (settings != null)
            {
                settings.DefaultValueHandling = DefaultValueHandling.Ignore;
                settings.ContractResolver = SnakeContractResolver.Create();
            }
            return settings;
        }

        public static void ApplySnakeJson(this IServiceCollection _)
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings().ApplySnakeJson();
        }

        public static void ApplySnakeBson(this IServiceCollection _)
        {
            var pack = new ConventionPack { SnakeCaseConvention.Create() };
            ConventionRegistry.Register("snake_case", pack, type => true);
        }

        public static IMongoCollection<T> GetCollection<T>(this IMongoDatabase database, MongoCollectionSettings settings = null)
        {
            return database.GetCollection<T>(ToRegular<T>(), settings);
        }

        public static bool HasAttribute<T>(this PropertyInfo info, out T value)
            where T : Attribute
        {
            value = info.GetCustomAttribute<T>();
            return value != null;
        }

        public static bool HasAttribute<T>(this MemberInfo info, out T value)
            where T : Attribute
        {
            value = info.GetCustomAttribute<T>();
            return value != null;
        }

        public static string ToRegular<T>()
        {
            return ToRegular(typeof(T).Name);
        }

        public static string ToRegular(this string value)
        {
            return Regex.Replace(value.Replace("`", ""), "([A-Z]{1,2}|[0-9]|`+)", "_$1").Trim('_').ToLower();
        }
    }
}
