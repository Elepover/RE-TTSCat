using System;

namespace Re_TTSCat
{
    public static partial class BaiduTTS
    {
        public static int ConvertSpeed(double originalSpeed)
        {
            if (originalSpeed == 0.0) return 5;
            if (originalSpeed < 0.0)
                return (int)Math.Round(0.5 * (originalSpeed + 10));
            else
                return (int)Math.Round(originalSpeed + 5);
        }
    }
}
