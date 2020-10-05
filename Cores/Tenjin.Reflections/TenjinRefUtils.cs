using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using Tenjin.Models.Attributes;
using Tenjin.Models.Entities;
using Tenjin.Models.Enums;

namespace Tenjin.Helpers
{
    public partial class TenjinRefUtils
    {
        public static bool GenerateIndexes<T>(out IndexKeysDefinition<T> value)
           where T : BaseEntity
        {
            var properties = typeof(T).GetProperties();
            if (!properties.Any())
            {
                value = null;
                return false;
            }
            else
            {
                value = Builders<T>.IndexKeys.Descending(x => x.Id);
                foreach (var property in properties)
                {
                    if (property.HasAttribute<BsonPropertyAttribute>(out var attr))
                    {
                        if (attr.BsonDirection == BsonDirection.DESC)
                        {
                            value = value.Descending(property.Name.ToRegular());
                        }
                        else if (attr.BsonDirection == BsonDirection.ASC)
                        {
                            value = value.Ascending(property.Name.ToRegular());
                        }
                    }
                }
                return true;
            }
        }

        public static bool GenerateTextIndexes<T>(out IndexKeysDefinition<T> value)
             where T : BaseEntity
        {
            var properties = typeof(T).GetProperties();
            var models = new List<IndexKeysDefinition<T>>();
            foreach (var property in properties)
            {
                if (property.HasAttribute<BsonPropertyAttribute>(out var attr) && attr?.IsTextSearch == true)
                {
                    models.Add(Builders<T>.IndexKeys.Text(property.Name.ToRegular()));
                }
            }
            value = Builders<T>.IndexKeys.Combine(models);
            return models.Count > 0;
        }
    }
}
