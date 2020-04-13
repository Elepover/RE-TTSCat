using System;

namespace Re_TTSCat.Data
{
    public partial class SecureRng
    {
        private static uint GetRandomUInt()
        {
            var randomBytes = GenerateRandomBytes(sizeof(uint));
            return BitConverter.ToUInt32(randomBytes, 0);
        }
    }
}
