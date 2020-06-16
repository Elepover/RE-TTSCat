using BilibiliDM_PluginFramework;
using Re_TTSCat.Data;

namespace Re_TTSCat
{
    public partial class Main : DMPlugin
    {
        public static string ProcessGift(DanmakuModel e, string template)
        {
            var rawDanmaku = Preprocess(template, e);
            return rawDanmaku
                .Replace("$GIFT", e.GiftName)
                .Replace("$COUNT", e.GiftCount.ToString())
                .Replace("$USER", e.UserName);
        }
        public static string ProcessGift(DanmakuModel e) => ProcessGift(e, Vars.CurrentConf.OnGift);
        public static string ProcessGift(ReceivedDanmakuArgs e) => ProcessGift(e.Danmaku, Vars.CurrentConf.OnGift);
        public static string ProcessGift(ReceivedDanmakuArgs e, string template) => ProcessGift(e.Danmaku, template);
    }
}