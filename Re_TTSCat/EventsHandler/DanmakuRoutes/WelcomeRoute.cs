using BilibiliDM_PluginFramework;
using System.Threading.Tasks;
using Re_TTSCat.Data;

namespace Re_TTSCat
{
    public partial class Main : DMPlugin
    {
        public static async Task WelcomeRoute(object sender, ReceivedDanmakuArgs e)
        {
            // check user eligibility
            if (!Conf.CheckUserEligibility(e)) return;
            Bridge.ALog("规则检查通过，准备朗读");
            await TTSPlayer.UnifiedPlay(Vars.CurrentConf.OnWelcome
                .Replace("$USER", e.Danmaku.UserName)
                , true
            );
        }
    }
}