using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Tenjin.Helpers.Enums;

namespace Tenjin.Helpers.Ciphers
{
    public class TenjinRijndael
    {
        public static string ComputeHash(string text, byte[] salts, HashType type = HashType.Md5)
        {
            salts = PrepareSalts(salts);
            var buffers = Encoding.UTF8.GetBytes(text).Concat(salts).ToArray();
            using var hash = GetHashAlgorithm(type);
            var computed = hash.ComputeHash(buffers).Concat(salts).ToArray();
            return Convert.ToBase64String(computed);
        }

        public static bool VerifyHash(string text, string hash, HashType type = HashType.Md5)
        {
            var buffers = Convert.FromBase64String(hash);
            int size = GetHashSizeBytes(type);
            if (buffers.Length < size)
                return false;
            var salts = buffers.Skip(size).ToArray();
            var expected = ComputeHash(text, salts, type);
            return hash == expected;
        }

        public static string EncryptRijndael(string value, string password = CipherConstants.RIJNDAEL_KEY)
        {
            try
            {
                var buffers = Encoding.UTF8.GetBytes(value);
                using var rijndael = CreateAes(password);
                using var transform = rijndael.CreateEncryptor();
                using var ms = new MemoryStream();
                using var cs = new CryptoStream(ms, transform, CryptoStreamMode.Write);
                cs.Write(buffers, 0, buffers.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch
            {
                return string.Empty;
            }
        }

        public static byte[] EncryptRijndael(byte[] buffers, string password = CipherConstants.RIJNDAEL_KEY)
        {
            if (buffers == null)
            {
                return default;
            }
            try
            {
                using var rijndael = CreateAes(password);
                using var transform = rijndael.CreateEncryptor();
                using var ms = new MemoryStream();
                using var cs = new CryptoStream(ms, transform, CryptoStreamMode.Write);
                cs.Write(buffers, 0, buffers.Length);
                cs.FlushFinalBlock();
                return ms.ToArray();
            }
            catch
            {
                return default;
            }
        }

        public static byte[] DecryptRijndael(byte[] buffers, string password = CipherConstants.RIJNDAEL_KEY)
        {
            if (buffers == null)
            {
                return default;
            }
            try
            {
                using var rijndael = CreateAes(password);
                using var transform = rijndael.CreateDecryptor();
                using var ms = new MemoryStream();
                using var cs = new CryptoStream(ms, transform, CryptoStreamMode.Write);
                cs.Write(buffers, 0, buffers.Length);
                cs.FlushFinalBlock();
                return ms.ToArray();
            }
            catch
            {
                return default;
            }
        }

        public static string DecryptRijndael(string value, string password = CipherConstants.RIJNDAEL_KEY)
        {
            try
            {
                var buffers = Convert.FromBase64String(value);
                using var rijndael = CreateAes(password);
                using var transform = rijndael.CreateDecryptor();
                using var ms = new MemoryStream();
                using var cs = new CryptoStream(ms, transform, CryptoStreamMode.Write);
                cs.Write(buffers, 0, buffers.Length);
                cs.FlushFinalBlock();
                return Encoding.UTF8.GetString(ms.ToArray());
            }
            catch
            {
                return string.Empty;
            }
        }

        #region Functions

        private static byte[] PrepareSalts(byte[] salts)
        {
            if (salts != null)
            {
                return salts;
            }
            var min = 4;
            var max = 8;
            var size = TenjinUtils.RandomInt32(min, max);
            salts = new byte[size];
            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(salts);
            return salts;
        }

        private static HashAlgorithm GetHashAlgorithm(HashType value)
        {
            return value switch
            {
                HashType.Sha1 => SHA1.Create(),
                HashType.Sha256 => SHA256.Create(),
                HashType.Sha384 => SHA384.Create(),
                HashType.Sha512 => SHA512.Create(),
                _ => MD5.Create(),
            };
        }

        private static int GetHashSizeBytes(HashType value)
        {
            var bits = value switch
            {
                HashType.Sha1 => 160,
                HashType.Sha256 => 256,
                HashType.Sha384 => 384,
                HashType.Sha512 => 512,
                _ => 128,
            };
            return bits / 8;
        }

        private static Aes CreateAes(string password)
        {
            var key = Encoding.UTF8.GetBytes(password);
            var rijndael = Aes.Create();
            rijndael.BlockSize = 128;
            rijndael.IV = key;
            rijndael.KeySize = 128;
            rijndael.Key = key;
            return rijndael;
        }

        #endregion
    }
}
