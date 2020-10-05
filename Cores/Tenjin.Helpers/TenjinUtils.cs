using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Tenjin.Helpers
{
    public partial class TenjinUtils
    {
        private static readonly Random random = new Random((int)DateTime.Now.Ticks);
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static bool IsNull(object value)
        {
            return value == null || value == DBNull.Value;
        }

        public static bool IsNotNull(object value)
        {
            return !IsNull(value);
        }

        public static bool IsStringEmpty(string value)
        {
            return (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(value.Trim()));
        }

        public static bool IsStringNotEmpty(string value)
        {
            return !IsStringEmpty(value);
        }

        public static bool IsArrayNotEmpty(IEnumerable<object> values)
        {
            if (IsNull(values)) return false;
            return values.Any();
        }

        public static bool IsArrayEmpty(IEnumerable<object> values)
        {
            return !IsArrayNotEmpty(values);
        }

        public static string EntryLocation
        {
            get
            {
                FileInfo file = new FileInfo(Assembly.GetEntryAssembly().Location);
                return file.DirectoryName;
            }
        }

        public static void CreateDirectory(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            CreateDirectory(di);
        }

        public static void CreateDirectory(DirectoryInfo current)
        {
            if (current.Exists) return;
            CreateDirectory(current.Parent);
            current.Create();
        }

        public static string RandomFile(string ext)
        {
            var name = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}{RandomString(8).ToLower()}";
            if (string.IsNullOrEmpty(ext) || string.IsNullOrEmpty(ext.TrimStart('.')))
            {
                return name;
            }
            return string.Concat(name, '.', ext.TrimStart('.'));
        }

        public static int RandomInt32(int from = 0, int to = 255)
        {
            return random.Next(from, to);
        }

        public static long RandomInt64(long from = 0, long to = 255)
        {
            ulong uRange = (ulong)(to - from);
            ulong ulongRand;
            do
            {
                byte[] buf = new byte[8];
                random.NextBytes(buf);
                ulongRand = (ulong)BitConverter.ToInt64(buf, 0);
            } while (ulongRand > ulong.MaxValue - ((ulong.MaxValue % uRange) + 1) % uRange);
            return (long)(ulongRand % uRange) + from;
        }

        public static string GetTypeFolder<T>()
        {
            var regex = new Regex("(?=[A-Z][^A-Z])");
            var name = typeof(T).Name;
            var splits = regex.Split(name).Select(x => x.ToLower()).Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return string.Join("-", splits);
        }
    }
}
