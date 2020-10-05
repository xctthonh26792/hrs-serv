using System;

namespace Tenjin.Helpers
{
    public class TenjinConstants
    {
        public const int TOKEN_TIMEOUT = 7;
        public const string PERMISSION_HEADER = "Permission";
        public const string BEARER_AUTHORIZATION_NAME = "Bearer";
        public const string AUTHORIZATION_HEADER_NAME = "Authorization";
        public static readonly DateTime MONGO_MIN_DATE = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }
}
