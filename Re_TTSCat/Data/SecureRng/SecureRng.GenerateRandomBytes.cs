namespace Re_TTSCat.Data
{
    public partial class SecureRng
    {
        private static byte[] GenerateRandomBytes(int bytesNumber)
        {
            byte[] buffer = new byte[bytesNumber];
            Csp.GetBytes(buffer);
            return buffer;
        }
    }
}
