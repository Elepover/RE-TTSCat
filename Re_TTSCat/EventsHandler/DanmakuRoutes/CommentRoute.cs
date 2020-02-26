using BilibiliDM_PluginFramework;
using System.Threading.Tasks;
using Re_TTSCat.Data;

namespace Re_TTSCat
{
    public partial class Main : DMPlugin
    {
        public async Task CommentRoute(object sender, ReceivedDanmakuArgs e)
        {
            // check user eligibility
            if (Vars.CurrentConf.BlockUID)
            {
                if (!Conf.CheckUserEligibility(e.Danmaku.UserID.ToString()))
                {
                    if (Vars.CurrentConf.DebugMode)
                    {
                        Bridge.ALog("忽略：用户已命中 UID 规则");
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
                        Bridge.ALog("忽略：用户已命中用户名规则");
                    }
                    return;
                }
            }
            // check content eligibility
            if (!Conf.CheckKeywordEligibility(e.Danmaku.CommentText))
            {
                if (Vars.CurrentConf.DebugMode)
                {
                    Bridge.ALog("忽略：弹幕已命中屏蔽规则");
                }
                return;
            }
            // check length rule
            if (!((e.Danmaku.CommentText.Length <= Vars.CurrentConf.MaximumDanmakuLength) && (e.Danmaku.CommentText.Length >= Vars.CurrentConf.MinimumDanmakuLength)))
            {
                if (Vars.CurrentConf.DebugMode)
                {
                    Bridge.ALog("忽略: 弹幕字数 (" + e.Danmaku.CommentText.Length + ") 不符合要求");
                }
                return;
            }
            if (Vars.CurrentConf.DebugMode)
            {
                Bridge.ALog("规则检查通过，准备朗读");
            }
            await TTSPlayer.UnifiedPlay(Vars.CurrentConf.OnDanmaku
                .Replace("$USER", e.Danmaku.UserName)
                .Replace("$DM", e.Danmaku.CommentText)
            );
        }
    }
}