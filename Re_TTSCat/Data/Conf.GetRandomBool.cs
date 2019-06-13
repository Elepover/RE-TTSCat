using System;

namespace Re_TTSCat.Data
{
    public partial class Conf
    {
        public static bool GetRandomBool(int truePercentage = 50) => (new Random()).NextDouble() < truePercentage / 100.0;
    }
}
