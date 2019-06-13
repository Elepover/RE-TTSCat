using BilibiliDM_PluginFramework;
using System.Threading.Tasks;
using Re_TTSCat.Data;

namespace Re_TTSCat
{
    public partial class Main : DMPlugin
    {
        public async Task WelcomeGuardRoute(object sender, ReceivedDanmakuArgs e)
        {
            // check user eligibility
            if (Vars.CurrentConf.BlockUID)
            {
                if (!Conf.CheckUserEligibility(e.Danmaku.UserID.ToString()))
                {
                    if (Vars.CurrentConf.DebugMode)
                    {
                        Bridge.Log("忽略：用户已命中 UID 规则");
                    }
                    return;
                }
            }
            else
            {
                if (!Conf.CheckUserEligibility(e.Danmaku.UserName))
                {
                    if (Vars.CurrentConf.DebugMode)
                    {
                        Bridge.Log("忽略：用户已命中用户名规则");
                    }
                    return;
                }
            }
            if (Vars.CurrentConf.DebugMode)
            {
                Bridge.Log("规则检查通过，准备朗读");
            }
            await TTSPlayer.UnifiedPlay(Vars.CurrentConf.OnWelcomeGuard
                .Replace("$USER", e.Danmaku.UserName)
            );
        }
    }
}