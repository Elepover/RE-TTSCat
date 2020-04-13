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

        public static bool CheckUserEligibility(ReceivedDanmakuArgs e)
        {
            if (Vars.CurrentConf.BlockUID)
            {
                if (!CheckUserEligibility(e.Danmaku.UserID.ToString()))
                {
                    Bridge.ALog("忽略：用户已命中 UID 规则");
                    return false;
                }
            }
            else
            {
                if (!CheckUserEligibility(e.Danmaku.UserName))
                {
                    Bridge.ALog("忽略：用户已命中用户名规则");
                    return false;
                }
            }
            return true;
        }
    }
}
