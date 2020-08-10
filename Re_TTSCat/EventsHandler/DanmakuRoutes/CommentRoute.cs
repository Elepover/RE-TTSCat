using BilibiliDM_PluginFramework;
using System.Threading.Tasks;
using Re_TTSCat.Data;
using System.Linq;

namespace Re_TTSCat
{
    public partial class Main : DMPlugin
    {
        public async Task CommentRoute(object sender, ReceivedDanmakuArgs e)
        {
            // check user eligibility
            if (!Conf.CheckUserEligibility(e)) return;
            // check content eligibility
            if (!Conf.CheckKeywordEligibility(e)) return;
            // check length rule
            if (!Conf.CheckDanmakuLength(e)) return;
            Bridge.ALog("规则检查通过，准备朗读");
            if (Vars.CurrentConf.VoiceReplyFirst)
            {
                var hitAnyRule = await TTSPlayer.PlayVoiceReply(e.Danmaku);
                if (!hitAnyRule || !Vars.CurrentConf.IgnoreIfHitVoiceReply)
                    await TTSPlayer.UnifiedPlay(ProcessDanmaku(e));
            }
            else
            {
                var hitAnyRule = Vars.CurrentConf.VoiceReplyRules.Any(x => x.Matches(e.Danmaku));
                if (!hitAnyRule || !Vars.CurrentConf.IgnoreIfHitVoiceReply)
                    await TTSPlayer.UnifiedPlay(ProcessDanmaku(e));
                await TTSPlayer.PlayVoiceReply(e.Danmaku);
            }
        }
    }
}