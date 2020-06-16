using BilibiliDM_PluginFramework;
using System.Threading.Tasks;
using Re_TTSCat.Data;

namespace Re_TTSCat
{
    public partial class Main : DMPlugin
    {
        public async Task GiftRoute(object sender, ReceivedDanmakuArgs e)
        {
            // check user eligibility
            if (!Conf.CheckUserEligibility(e)) return;
            // check gift eligibility
            if (!Conf.CheckGiftEligibility(e)) return;
            Bridge.ALog("规则检查通过，准备朗读");
            await TTSPlayer.PlayVoiceReply(e.Danmaku);
            await TTSPlayer.UnifiedPlay(ProcessGift(e));
        }
    }
}