using MongoDB.Bson;

namespace Tenjin.Models
{
    public static class Extensions
    {
        public static ObjectId ToObjectId(this string value)
        {
            return ObjectId.TryParse(value, out var @object) ? @object : ObjectId.Empty;
        }

        public static bool IsObjectIdEmpty(this string value)
        {
            var @object = value.ToObjectId();
            return @object == ObjectId.Empty;
        }
    }
}
