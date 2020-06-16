using BilibiliDM_PluginFramework;
using Re_TTSCat.Data;

namespace Re_TTSCat
{
    public partial class Main : DMPlugin
    {
        public static string ProcessSuperChat(ReceivedDanmakuArgs e)
        {
            var rawDanmaku = Preprocess(Vars.CurrentConf.OnSuperChat, e);
            return rawDanmaku
                .Replace("$PRICE", e.Danmaku.Price.ToString())
                .Replace("$USER", e.Danmaku.UserName)
                .Replace("$DM", e.Danmaku.CommentText);
        }
    }
}