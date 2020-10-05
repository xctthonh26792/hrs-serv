using Microsoft.AspNetCore.DataProtection;

namespace Tenjin.Sys.Helpers
{
    public class TenjinDataProtector : IDataProtector
    {
        public IDataProtector CreateProtector(string purpose)
        {
            return new TenjinDataProtector();
        }

        public byte[] Protect(byte[] plaintext)
        {
            return plaintext;
        }

        public byte[] Unprotect(byte[] protectedData)
        {
            return protectedData;
        }
    }
}
