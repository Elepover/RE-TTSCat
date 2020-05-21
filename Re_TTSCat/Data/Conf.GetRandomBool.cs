namespace Re_TTSCat.Data
{
    public partial class Conf
    {
        public static bool GetRandomBool(int truePercentage = 50)
        {
            return SecureRng.Next(0, 101) <= truePercentage;
        }
    }
}
