using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Tenjin.Helpers.Ciphers;
using Tenjin.Helpers.Enums;

namespace Tenjin.Helpers
{
    public static partial class TenjinExtensions
    {
        public static string Shorten(this string text, int shortenLength, bool isCheckLastDot = false,
            bool isRemoveHtml = true)
        {
            if (string.IsNullOrEmpty(text) || (shortenLength <= 0)) return text;
            if (isRemoveHtml) text = Regex.Replace(text.Trim(), @"\<[^\>]*\>", string.Empty);
            var words = text.Split(new[] { ' ' }, shortenLength + 1, StringSplitOptions.RemoveEmptyEntries);
            var output = words.Count() > shortenLength
                ? string.Join(" ", words.Take(shortenLength))
                : string.Join(" ", words);
            if (!isCheckLastDot) return output;
            var lastDotIndex = text.LastIndexOf(".", StringComparison.Ordinal);
            return lastDotIndex > 0 ? output.Substring(0, lastDotIndex + 1) : output;
        }

        public static DateTime OneMonthAgo(this DateTime dateTime)
        {
            return dateTime.AddMonths(-1);
        }

        public static DateTime OneWeekAgo(this DateTime dateTime)
        {
            return dateTime.AddDays(-7);
        }

        public static string RemoveHtmlTags(this string text)
        {
            return string.IsNullOrEmpty(text) ? string.Empty : Regex.Replace(text, "\\<[^\\>]*\\>", string.Empty);
        }

        public static string CleanUpHtml(this string text)
        {
            return string.IsNullOrEmpty(text) ? string.Empty : text.CleanMsWordTags().CleanInlineCssStyle();
        }

        public static string CleanMsWordTags(this string text)
        {
            return Regex.Replace(text, @"<!\-\-\[if gte mso \d+\]>(.*?)<!\[endif\]\-\->", "",
                RegexOptions.CultureInvariant | RegexOptions.Singleline);
        }

        public static string CleanInlineCssStyle(this string text)
        {
            text = Regex.Replace(text, @"style='{1}(.*?)'{1}", "");
            text = Regex.Replace(text, @"style=""{1}(.*?)""{1}", "");
            return text;
        }

        public static string EncryptRijndael(this string value, string key = CipherConstants.RIJNDAEL_KEY)
        {
            return TenjinRijndael.EncryptRijndael(value, key);
        }

        public static string DecryptRijndael(this string value, string key = CipherConstants.RIJNDAEL_KEY)
        {
            return TenjinRijndael.DecryptRijndael(value, key);
        }

        public static string RijndaelHash(this string inputString, HashType hashType = HashType.Md5)
        {
            return TenjinRijndael.ComputeHash(inputString, null, hashType);
        }

        public static bool VerifyRijndaelHash(this string plainText, string hashValue, HashType hashType = HashType.Md5)
        {
            try
            {
                return TenjinRijndael.VerifyHash(plainText, hashValue, hashType);
            }
            catch
            {
                return false;
            }
        }

        public static string NormalizeString(this string strInput)
        {
            try
            {
                return ReplaceWithRegex(strInput, @"\s+", " ");
            }
            catch (Exception)
            {
                return strInput;
            }
        }

        public static string ReplaceWithRegex(this string strInput, string pattern, string replacement)
        {
            try
            {
                if (string.IsNullOrEmpty(strInput))
                    return string.Empty;
                strInput = strInput.Trim();
                return Regex.Replace(strInput, pattern, replacement);
            }
            catch (Exception)
            {
                return strInput;
            }
        }

        public static string ViToEn(this string unicodeString, bool special = true)
        {
            if (string.IsNullOrEmpty(unicodeString))
                return string.Empty;

            try
            {
                unicodeString = unicodeString.NormalizeString();
                //Remove Vietmamese character
                unicodeString = unicodeString.ToLower().Trim();
                unicodeString = Regex.Replace(unicodeString, "[áàảãạâấầẩẫậăắằẳẵặ]", "a");
                unicodeString = Regex.Replace(unicodeString, "[éèẻẽẹêếềểễệ]", "e");
                unicodeString = Regex.Replace(unicodeString, "[iíìỉĩị]", "i");
                unicodeString = Regex.Replace(unicodeString, "[óòỏõọơớờởỡợôốồổỗộ]", "o");
                unicodeString = Regex.Replace(unicodeString, "[úùủũụưứừửữự]", "u");
                unicodeString = Regex.Replace(unicodeString, "[yýỳỷỹỵ]", "y");
                unicodeString = Regex.Replace(unicodeString, "[đ]", "d");

                //Replace space
                //unicodeString = unicodeString.Replace(" ", "-");

                //Remove special character
                if (special)
                {
                    unicodeString = Regex.Replace(unicodeString, "[\"`~!@#$%^&*()-+=?/>.<,{}[]|]\\]", "");
                }
                unicodeString =
                    unicodeString.Replace("̀", "").Replace("̣", "").Replace("̉", "").Replace("̃", "").Replace("́", "");
                //Process for VNI.
                return unicodeString;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string ToSeoUrl(this string urlToEncode, bool viToEn = true)
        {
            if (string.IsNullOrEmpty(urlToEncode))
                return string.Empty;

            try
            {
                urlToEncode = urlToEncode.Trim().ToLower();
                if (viToEn)
                    urlToEncode = ViToEn(urlToEncode);
                var url = new StringBuilder();
                foreach (var ch in urlToEncode)
                    switch (ch)
                    {
                        case ' ':
                            url.Append('-');
                            break;
                        case '&':
                            url.Append("-");
                            break;
                        case ':':
                            url.Append("-");
                            break;
                        case '\'':
                            break;
                        default:
                            if (((ch >= '0') && (ch <= '9')) ||
                                ((ch >= 'a') && (ch <= 'z')))
                                url.Append(ch);
                            else
                                url.Append('-');
                            break;
                    }
                var ret = url.ToString();
                if (ret.Length > 240)
                    ret = ret.Substring(0, 240);
                while (ret.IndexOf("--", StringComparison.CurrentCulture) != -1)
                    ret = ret.Replace("--", "-");
                if (ret.IndexOf("-", StringComparison.CurrentCulture) == 0)
                    ret = ret.Remove(0, 1);
                if ((ret.LastIndexOf("-", StringComparison.CurrentCulture) == ret.Length - 1) && (ret.Length > 0))
                    ret = ret.Remove(ret.Length - 1, 1);
                return ret;
            }
            catch (Exception)
            {
                return urlToEncode;
            }
        }

        public static string ToUtf8String(this string original, string charset = "")
        {
            try
            {
                if (string.IsNullOrEmpty(charset)) return original;
                var encoding = Encoding.GetEncoding(charset);
                var bytes = encoding.GetBytes(original);
                var ubytes = Encoding.Convert(encoding, Encoding.UTF8, bytes);
                return WebUtility.UrlDecode(Encoding.UTF8.GetString(ubytes));
            }
            catch
            {
                return original;
            }
        }

        public static IEnumerable<T> Random<T>(this IEnumerable<T> values, int size)
        {
            return values.Select(x => new
            {
                Guid = Guid.NewGuid(),
                Model = x
            }).OrderBy(x => x.Guid).Take(size).Select(x => x.Model);
        }

        public static IEnumerable<T> Random<T>(this IEnumerable<T> values, long size)
        {
            return values.Select(x => new
            {
                Guid = Guid.NewGuid(),
                Model = x
            }).OrderBy(x => x.Guid).Take((int)size).Select(x => x.Model);
        }

        public static string GetPeriod(DateTime date)
        {
            if (DateTime.Now <= date) return " ... ";
            if (DateTime.Now.AddMonths(-1) > date) return "> 1 tháng trước";
            if (DateTime.Now.AddDays(-1) > date) return (DateTime.Now - date).Days + " ngày trước";
            if (DateTime.Now.AddHours(-1) > date) return (DateTime.Now - date).Hours + " giờ trước";
            if (DateTime.Now.AddMinutes(-1) > date) return (DateTime.Now - date).Minutes + " phút trước";
            return (DateTime.Now - date).Seconds + " giây trước";
        }

        public static string[] ParseCsvToArray(this string csv)
        {
            try
            {
                return Regex.Split(csv, @"\s*,\s*");
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static bool IsValidMailAddress(this string email, bool required = false)
        {
            if (string.IsNullOrEmpty(email)) return !required;
            return Regex.IsMatch(email,
                @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        public static int IndexOf<T>(this List<T> items, Func<T, bool> filter)
        {
            var view = items.FirstOrDefault(filter);
            return Equals(view, null) ? -1 : items.IndexOf(view);
        }

        public static int IndexOf<T>(this IEnumerable<T> items, Func<T, bool> filter)
        {
            return items.ToList().IndexOf(filter);
        }

        public static IEnumerable<IEnumerable<TSource>> GroupSize<TSource>
                  (this IEnumerable<TSource> elements, int size)
        {
            return elements.Select((element, index) => new { Group = index / size, Element = element })
                        .GroupBy(x => x.Group)
                        .Select(x => x.Select(y => y.Element));
        }

        public static IEnumerable<string> ToKeywords(this string value, int msize = 2)
        {
            var texts = value.Split(' ', '-');
            var list = new List<string>();
            foreach (var text in texts)
            {
                if (string.IsNullOrEmpty(text)) continue;
                list.Add(text);
                if (text.Length <= msize) continue;

                var ltext = text;
                for (var i = 0; i <= text.Length; i++)
                {
                    ltext = ltext.Substring(1);
                    list.Add(ltext);
                    if (ltext.Length <= msize) break;
                }
                var rtext = text;
                for (var i = 0; i <= text.Length; i++)
                {
                    rtext = rtext.Substring(0, rtext.Length - 1);
                    list.Add(rtext);
                    if (rtext.Length <= msize) break;
                }
            }
            return list.Distinct();
        }
    }
}
