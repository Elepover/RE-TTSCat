using BilibiliDM_PluginFramework;
using System.Threading.Tasks;
using Re_TTSCat.Data;

namespace Re_TTSCat
{
    public partial class Main : DMPlugin
    {
        public async Task SuperChatRoute(object sender, ReceivedDanmakuArgs e)
        {
            // check user eligibility
            if (!Conf.CheckUserEligibility(e)) return;
            // check content eligibility
            if (!Conf.CheckKeywordEligibility(e)) return;
            // check length rule
            if (!Conf.CheckDanmakuLength(e)) return;
            Bridge.ALog("规则检查通过，准备朗读");
            await TTSPlayer.UnifiedPlay(ProcessSuperChat(e), Vars.CurrentConf.SuperChatIgnoreRandomDitch);
        }
    }
}