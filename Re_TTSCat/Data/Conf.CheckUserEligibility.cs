using BilibiliDM_PluginFramework;

namespace Re_TTSCat.Data
{
    public partial class Conf
    {
        public static bool CheckUserEligibility(string content)
        {
            switch (Vars.CurrentConf.BlockMode)
            {
                default:
                    return true;
                case 1:
                    return Vars.CurrentConf.BlackList.Contains(content) ? false : true;
                case 2:
                    return Vars.CurrentConf.WhiteList.Contains(content) ? true : false;
            }
        }
    }
}
