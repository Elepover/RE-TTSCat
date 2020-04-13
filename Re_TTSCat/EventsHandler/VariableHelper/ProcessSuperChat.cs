using BilibiliDM_PluginFramework;
using Re_TTSCat.Data;

namespace Re_TTSCat
{
    public partial class Main : DMPlugin
    {
        public string ProcessSuperChat(ReceivedDanmakuArgs e)
        {
            var rawDanmaku = Preprocess(Vars.CurrentConf.OnSuperChat, e);
            return rawDanmaku
                .Replace("$USER", e.Danmaku.UserName)
                .Replace("$DM", e.Danmaku.CommentText);
        }
    }
}