using System;
using System.Globalization;

namespace Tenjin.Helpers
{
    public static class TenjinConverts
    {
        public static string GetString(object value, string defaultValue = default)
        {
            if (TenjinUtils.IsNull(value)) return defaultValue;
            if (value is string) return ((string)value).Trim();
            return value.ToString().Trim();
        }

        public static int GetInt32(object value, int defaultValue = default)
        {
            if (TenjinUtils.IsNull(value)) return defaultValue;
            if (value is int) return (int)value;
            return int.TryParse(GetString(value), out int r) ? r : defaultValue;
        }

        public static int? GetNullableInt32(object value, int? defaultValue = default)
        {
            if (TenjinUtils.IsNull(value)) return defaultValue;
            if (value is int) return (int)value;
            return int.TryParse(GetString(value), out int r) ? r : defaultValue;
        }

        public static double GetDouble(object value, double defaultValue = default)
        {
            if (TenjinUtils.IsNull(value)) return defaultValue;
            if (value is double) return (double)value;
            return double.TryParse(GetString(value), out double r) ? r : defaultValue;
        }

        public static decimal GetDecimal(object value, decimal defaultValue = default)
        {
            if (TenjinUtils.IsNull(value)) return defaultValue;
            if (value is decimal) return (decimal)value;
            return decimal.TryParse(GetString(value), out decimal r) ? r : defaultValue;
        }

        public static double? GetNullableDouble(object value, double? defaultValue = default)
        {
            if (TenjinUtils.IsNull(value)) return defaultValue;
            if (value is double) return (double)value;
            return double.TryParse(GetString(value), out double r) ? r : defaultValue;
        }

        public static decimal? GetNullableDecimal(object value, decimal? defaultValue = default)
        {
            if (TenjinUtils.IsNull(value)) return defaultValue;
            if (value is decimal) return (decimal)value;
            return decimal.TryParse(GetString(value), out decimal r) ? r : defaultValue;
        }

        public static DateTime GetDateTimeExact(object value, string format = "dd/MM/yyyy", DateTime defaultValue = default)
        {
            if (TenjinUtils.IsNull(value) || TenjinUtils.IsStringEmpty(format)) return defaultValue;
            if (value is DateTime) return (DateTime)value;
            return DateTime.TryParseExact(GetString(value), format, null, DateTimeStyles.None, out DateTime r) ? r : defaultValue;
        }

        public static DateTime? GetNullableDateTimeExact(object value, string format = "dd/MM/yyyy", DateTime? defaultValue = default)
        {
            if (TenjinUtils.IsNull(value) || TenjinUtils.IsStringEmpty(format)) return defaultValue;
            if (value is DateTime) return (DateTime)value;
            return DateTime.TryParseExact(GetString(value), format, null, DateTimeStyles.None, out DateTime r) ? r : defaultValue;
        }

        public static DateTime GetDateTime(object value, DateTime defaultValue = default)
        {
            if (TenjinUtils.IsNull(value)) return defaultValue;
            if (value is DateTime) return (DateTime)value;
            return DateTime.TryParse(GetString(value), out DateTime r) ? r : defaultValue;
        }

        public static DateTime? GetNullableDateTime(object value, DateTime? defaultValue = default)
        {
            if (TenjinUtils.IsNull(value)) return defaultValue;
            if (value is DateTime) return (DateTime)value;
            return DateTime.TryParse(GetString(value), out DateTime r) ? r : defaultValue;
        }

        public static T GetEnum<T>(object value) where T : struct
        {
            if (value is T) return (T)value;
            return Enum.TryParse(GetString(value), out T result) ? result : default;
        }
    }
}
