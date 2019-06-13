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
                    if (Vars.CurrentConf.BlackList.Contains(content))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                case 2:
                    if (Vars.CurrentConf.WhiteList.Contains(content))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
            }
        }
    }
}
