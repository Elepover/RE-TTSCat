using BilibiliDM_PluginFramework;
using Re_TTSCat.Data;

namespace Re_TTSCat
{
    public partial class Main : DMPlugin
    {
        public static string ProcessDanmaku(DanmakuModel e, string template)
        {
            var rawDanmaku = Preprocess(template, e);
            return rawDanmaku
                .Replace("$USER", e.UserName)
                .Replace("$DM", e.CommentText);
        }
        public static string ProcessDanmaku(DanmakuModel e) => ProcessDanmaku(e, Vars.CurrentConf.OnDanmaku);
        public static string ProcessDanmaku(ReceivedDanmakuArgs e) => ProcessDanmaku(e.Danmaku);
        public static string ProcessDanmaku(ReceivedDanmakuArgs e, string template) => ProcessDanmaku(e.Danmaku, template);
    }
}