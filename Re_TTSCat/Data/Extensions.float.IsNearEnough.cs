using System;

namespace Re_TTSCat.Data
{
    public static partial class Extensions
    {
        public static bool IsNearEnough(this float _float, float comparison, float epsilon = float.Epsilon) => Math.Abs(_float - comparison) < epsilon;
    }
}
