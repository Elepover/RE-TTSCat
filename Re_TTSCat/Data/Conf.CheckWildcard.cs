using Microsoft.VisualBasic.CompilerServices;

namespace Re_TTSCat.Data
{
    public partial class Conf
    {
        public static bool CheckWildcard(string pattern, string source)
        {
            return LikeOperator.LikeString(source, pattern, Microsoft.VisualBasic.CompareMethod.Binary);
        }
    }
}
