using System.Security.Cryptography;

namespace Re_TTSCat.Data
{
    public partial class SecureRng
    {
        private static RNGCryptoServiceProvider Csp => new RNGCryptoServiceProvider();
    }
}
