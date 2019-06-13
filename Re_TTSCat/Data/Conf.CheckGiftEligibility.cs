using BilibiliDM_PluginFramework;

namespace Re_TTSCat.Data
{
    public partial class Conf
    {
        public static bool CheckGiftEligibility(string content)
        {
            switch (Vars.CurrentConf.GiftBlockMode)
            {
                default:
                    return true;
                case 1:
                    if (Vars.CurrentConf.GiftBlackList.Contains(content))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                case 2:
                    if (Vars.CurrentConf.GiftWhiteList.Contains(content))
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
