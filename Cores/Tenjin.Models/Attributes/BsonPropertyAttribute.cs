using System;
using Tenjin.Models.Enums;

namespace Tenjin.Models.Attributes
{
    public class BsonPropertyAttribute : Attribute
    {
        public BsonPropertyAttribute() : this(false, BsonDirection.NONE)
        {
        }
        public BsonPropertyAttribute(bool search = false, BsonDirection direction = BsonDirection.NONE)
        {
            IsTextSearch = search;
            BsonDirection = direction;
        }

        public BsonPropertyAttribute(BsonDirection direction = BsonDirection.NONE, bool search = false)
        {
            IsTextSearch = search;
            BsonDirection = direction;
        }

        public BsonDirection BsonDirection { get; private set; }
        public bool IsTextSearch { get; private set; }
    }
}
